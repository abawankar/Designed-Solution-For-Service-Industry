using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Controllers
{
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/

        //[Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR007"))
            {
                CompanyRepository dal = new CompanyRepository();

                ViewBag.bran = new SelectList(dal.BranchList().OrderBy(x => x.Name), "Id", "Name");

                ViewBag.dept = new SelectList(dal.DepartmentList().Take(0).OrderBy(x => x.Name), "Id", "Name");

                return View();
            }
            else
            {
                return View("_unAuthorised");
            }
        }

        //List all employee details
        [HttpPost]
        public JsonResult List(string col = null, int branId = 0, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                EmployeeRepository dal = new EmployeeRepository();
                List<EmployeeModel> model = new List<EmployeeModel>();
                if (!string.IsNullOrEmpty(name))
                {
                    switch (col)
                    {
                        case "search":
                            model = dal.GetAll().Where(x => x.EmpName.ToLower().Contains(name.ToLower())).ToList();
                            break;
                        case "dept":
                            {
                                if (name != "0")
                                {
                                    model = dal.GetAll().Where(x => x.Branch.Id == branId && x.Department.Id == Convert.ToInt32(name)).ToList();
                                }
                                else
                                {
                                    model = dal.GetAll().Where(x => x.Branch.Id == branId).ToList();
                                }
                                break;
                            }
                        default:
                            model = dal.GetAll().ToList();
                            break;
                    }

                }
                else
                {
                    if (branId != 0 && col == "branch")
                    {
                        model = dal.GetAll().Where(x => x.Branch.Id == branId).ToList();
                    }
                    else
                    {
                        model = dal.GetAll().ToList();
                    }


                }
                int count = model.Count;
                List<EmployeeModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new employee
        [HttpPost]
        public JsonResult Create(EmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                if(User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR005"))
                {
                    EmployeeRepository dal = new EmployeeRepository();
                    int empid = dal.GetByName(model.AppLogin);
                    if (empid == 0)
                    {
                        dal.Insert(model);
                        return Json(new { Result = "OK", Record = model });
                    }
                    else
                    {
                        return Json(new { Result = "Error", Message = "Employee Already Added" });
                    }
                }
                else
                {
                    return Json(new { Result = "Error", Message = "Not authorised to do this action" });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update employee details
        [HttpPost]
        public JsonResult Update(EmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR005"))
                {
                    EmployeeRepository dal = new EmployeeRepository();
                    dal.Edit(model);
                    EmployeeModel Model = dal.GetById(model.Id);
                    return Json(new { Result = "OK", Record = Model });
                }
                else
                {
                    return Json(new { Result = "Error", Message = "Not authorised to do this action" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetEmployeeOptions()
        {
            try
            {
                EmployeeRepository dal = new EmployeeRepository();
                List<EmployeeModel> model = new List<EmployeeModel>();
                if (User.IsInRole("User"))
                {
                    model = dal.GetAll().Where(x => x.AppLogin.ToLower() == User.Identity.Name.ToLower()).ToList();
                }
                else
                {
                    if (User.IsInRole("Manager"))
                    {
                        int groupId = dal.GetManagerDept(User.Identity.Name.ToUpper());
                        model = dal.GetAll().Where(x => x.GroupId == groupId).ToList();
                    }
                    else
                    {
                        model = dal.GetAll().OrderBy(x => x.EmpName).ToList();
                    }
                }
                var list = model.Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetEmployeeList()
        {
            try
            {
                EmployeeRepository dal = new EmployeeRepository();
                List<EmployeeModel> model = new List<EmployeeModel>();
                model = dal.GetAll().OrderBy(x => x.EmpName).ToList();
                var list = model.Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetEmployee()
        {
            try
            {
                EmployeeRepository dal = new EmployeeRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.EmpName)
                           select new { Id = m.Id, Name = m.EmpName };
                return Json(list, "employee", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public int GetDeptid(string applogin)
        {
            EmployeeRepository dal = new EmployeeRepository();
            return dal.GetBranchId(applogin.ToUpper());
        }

        public ActionResult ExportData(int branch = 0)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Id", typeof(int)));
            dt.Columns.Add(new DataColumn("Code", typeof(string)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("AppLogin", typeof(string)));
            dt.Columns.Add(new DataColumn("Mailid", typeof(string)));
            dt.Columns.Add(new DataColumn("Company", typeof(string)));
            dt.Columns.Add(new DataColumn("Branch", typeof(string)));
            dt.Columns.Add(new DataColumn("Department", typeof(string)));
            dt.Columns.Add(new DataColumn("Group", typeof(string)));

            GridView gv = new GridView();
            EmployeeRepository dal = new EmployeeRepository();
            string branchName = "";
            foreach (var item in branch == 0 ? dal.GetAll() : dal.GetAll().Where(x => x.Branch.Id == branch))
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = item.Id;
                dr["Code"] = item.EmpCode;
                dr["Name"] = item.EmpName;
                dr["AppLogin"] = item.AppLogin;
                dr["Mailid"] = item.MailId;
                dr["Company"] = item.Company.Name;
                dr["Branch"] = item.Branch.Name;
                branchName = item.Branch.Name;
                dr["Department"] = item.Department.Name;
                dr["Group"] = item.Group.Name;
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = branch == 0 ? "Employee.xls" : "Employee-" + branchName + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);

            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }

        public ActionResult AddEmpRights(string id, string list)
        {
            if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR006"))
            {
                EmployeeRepository.AddEmpRights(Convert.ToInt32(id), list);
                return RedirectToAction("Index");
            }
            else
            {
                return View("_unAuthorised");
            }
        }

        [HttpPost]
        public JsonResult GetEmpRights(int id = 0)
        {
            EmployeeRepository dal = new EmployeeRepository();
            try
            {
                var data = dal.GetById(id).EmpRights.ToList();

                return Json(new { Result = "OK", Records = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListOfEmpRights(int id = 0, int jtStartIndex = 0, int jtPageSize = 0)
        {
            EmployeeRepository dal = new EmployeeRepository();
            EmpRightsRepository cdal = new EmpRightsRepository();
            try
            {
                var empRights = dal.GetById(id).EmpRights;
                var rightId = from c in empRights
                              select c.Id;

                var list = cdal.GetAll().Where(c => !rightId.Contains(c.Id)).ToList();

                int count = list.Count;
                var Model1 = list.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetEmpRightsOptions(int id = 0)
        {
            EmployeeRepository dal = new EmployeeRepository();
            EmpRightsRepository cdal = new EmpRightsRepository();
            try
            {
                if (id == 0)
                {
                    var curr = cdal.GetAll()
                               .Select(c => new { DisplayText = c.Description, Value = c.Id });
                    return Json(new { Result = "OK", Options = curr });
                }
                else
                {
                    var curr = dal.GetById(id).EmpRights
                               .Select(c => new { DisplayText = c.Description, Value = c.Id });
                    return Json(new { Result = "OK", Options = curr });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //View Employee Rights Page, Only for Admin
        public ActionResult EmpRightsList(int id)
        {
            if (User.IsInRole("Admin"))
            {
                EmployeeRepository dal = new EmployeeRepository();
                ViewBag.Employee = new SelectList(dal.GetEmpById(id), "Id", "EmpName");
                return PartialView();
            }
            else
            {
                return View("Security");
            }
        }

        public JsonResult DeleteRights(EmpRightsModel model, int empId)
        {
            try
            {

                if (User.IsInRole("Admin") || EmpRightsRepository.RightList(User.Identity.Name).Contains("R009"))
                {
                    EmployeeRepository.DeleteRights(empId, model.Id);
                    return Json(new { Result = "OK", Record = model });
                }
                else
                {
                    return Json(new { Result = "Error", Message = "Sorry, You are not Authorized to do this action" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

    }
}
