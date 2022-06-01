using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Controllers
{
    public class CompanyController : Controller
    {
        //
        // GET: /Company/

        //[Authorize(Roles = "Admin"), HandleError]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR001"))
            {
                return View();
            }
            else
            {
                return View("_unAuthorised");
            }
                
        }

        #region ---- Company ----

        //List all company details
        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                CompanyRepository dal = new CompanyRepository();
                List<CompanyModel> model = new List<CompanyModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().ToList();
                }
                int count = model.Count;
                List<CompanyModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new company
        [HttpPost]
        public JsonResult Create(CompanyModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR002"))
                {
                    CompanyRepository dal = new CompanyRepository();
                    dal.Insert(model);
                    return Json(new { Result = "OK", Record = model });
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

        //Update company details
        [HttpPost]
        public JsonResult Update(CompanyModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR002"))
                {
                    CompanyRepository dal = new CompanyRepository();
                    dal.Edit(model);
                    return Json(new { Result = "OK", Record = model });
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

        #endregion

        #region ---- Branches ----

        //Add Company Branches
        [HttpPost]
        public JsonResult AddBranch(BranchModel model, int compId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR003"))
                {
                    CompanyRepository dal = new CompanyRepository();
                    dal.AddBranches(model, compId);
                    return Json(new { Result = "OK", Record = model });
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

        //Edit company Branches
        [HttpPost]
        public JsonResult EditBranch(BranchModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR003"))
                {
                    CompanyRepository dal = new CompanyRepository();
                    dal.EditBranches(model);
                    return Json(new { Result = "OK", Record = model });
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

        //List all company branches 
        [HttpPost]
        public JsonResult ListBranches(int compId)
        {
            try
            {
                CompanyRepository dal = new CompanyRepository();
                var model = dal.BranchList(compId);
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public ActionResult NatureList(int id)
        {
            CompanyRepository dal = new CompanyRepository();
            ViewBag.branch = new SelectList(dal.GetById(id).Branches, "Id", "Name");
            return PartialView();
        }

        public ActionResult AddNature(int branchId, string list)
        {
            CompanyRepository dal = new CompanyRepository();
            dal.AddNature(branchId, list);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ListOfNature(int id = 0)
        {
            CompanyRepository dal = new CompanyRepository();
            BusinessNatureRepository natdal = new BusinessNatureRepository();
            try
            {
                var branch = dal.BranchById(id).Nature;
                var natureId = from c in branch
                               select c.Id;

                var list = natdal.GetAll().Where(c => !natureId.Contains(c.Id)).ToList();

                return Json(new { Result = "OK", Records = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetNature(int branID = 0)
        {
            CompanyRepository dal = new CompanyRepository();
            try
            {
                var data = dal.BranchById(branID).Nature.ToList();

                return Json(new { Result = "OK", Records = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region ---- Department ----

        //List all branch department 
        [HttpPost]
        public JsonResult ListDepartment(int branId)
        {
            try
            {
                CompanyRepository dal = new CompanyRepository();
                var model = dal.DepartmentList(branId);
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Add branch department
        [HttpPost]
        public JsonResult AddDepartment(DepartmentModel model, int branId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if(User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR004"))
                {
                    CompanyRepository dal = new CompanyRepository();
                    dal.AddDepartment(model, branId);
                    return Json(new { Result = "OK", Record = model });
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

        //Edit branch department
        [HttpPost]
        public JsonResult EditDepartment(DepartmentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR004"))
                {
                    CompanyRepository dal = new CompanyRepository();
                    dal.EditDepartment(model);
                    return Json(new { Result = "OK", Record = model });
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

        #endregion

        #region -- Option fild ---

        // Get company options
        [HttpPost]
        public JsonResult GetCompanyOptions()
        {
            try
            {
                CompanyRepository dal = new CompanyRepository();
                var list = dal.GetAll()
                                .Select(c => new { DisplayText = c.Code, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        // Get branch options
        [HttpPost]
        public JsonResult GetBranchOptions(int compId = 0)
        {
            CompanyRepository dal = new CompanyRepository();
            try
            {

                if (compId != 0)
                {
                    var data = dal.BranchList(compId)
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }
                else
                {
                    var data = dal.BranchList()
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetBranch()
        {
            CompanyRepository dal = new CompanyRepository();
            try
            {
                var data = dal.BranchList(1)
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SelectBranch()
        {
            CompanyRepository dal = new CompanyRepository();
            try
            {
                var data = dal.SelectBranch(1).OrderBy(x => x.Name)
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetEmployeeOptions(int branid = 0)
        {
            EmployeeRepository dal = new EmployeeRepository();
            try
            {

                if (branid != 0)
                {
                    var data = dal.GetAll().Where(x => x.Branch.Id == branid)
                               .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }
                else
                {
                    var data = dal.GetAll()
                               .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetNBOEmployee(int branid = 0)
        {
            EmployeeRepository dal = new EmployeeRepository();
            try
            {
                string[] notIn = { "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };

                if (branid != 0)
                {
                    var data = dal.GetAll().Where(x => x.Branch.Id == branid &&
                                !notIn.Contains(x.Group.Id.ToString()))
                               .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }
                else
                {
                    var data = dal.GetAll()
                               .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SelectNBOEmployee(int branid = 0)
        {
            EmployeeRepository dal = new EmployeeRepository();
            try
            {
                string[] notIn = { "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };

                if (branid != 0)
                {
                    var data = dal.GetAll().Where(x => x.Branch.Id == branid &&
                                !notIn.Contains(x.Group.Id.ToString())).OrderBy(x => x.EmpName)
                               .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    var data1 = dal.SelectEmployee()
                                .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.Concat(data1).OrderBy(x => x.DisplayText) });
                }
                else
                {
                    var data = dal.GetAll().OrderBy(x => x.EmpName)
                               .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    var data1 = dal.SelectEmployee()
                                .Select(c => new { DisplayText = c.EmpName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.Concat(data1).OrderBy(x => x.DisplayText) });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetEmployee(int branid)
        {
            EmployeeRepository dal = new EmployeeRepository();
            var model = dal.GetAll().Where(x => x.Branch.Id == branid).ToList();
            return Json(new { Result = model.OrderBy(x => x.EmpName) });
        }

        [HttpPost]
        public JsonResult GetEmployeeNBO(int branid)
        {
            string[] notIn = { "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };
            EmployeeRepository dal = new EmployeeRepository();
            var model = dal.GetAll().Where(x => !notIn.Contains(x.Group.Id.ToString())).ToList();
            if (branid != 0)
            {
                model = model.Where(x => x.Branch.Id == branid).ToList();
            }
            return Json(new { Result = model.OrderBy(x => x.EmpName) });
        }

        // Get department options
        [HttpPost]
        public JsonResult GetDeptOptions(int branchId = 0)
        {
            CompanyRepository dal = new CompanyRepository();
            try
            {

                if (branchId != 0)
                {
                    var data = dal.DepartmentList(branchId)
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }
                else
                {
                    var data = dal.DepartmentList()
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult GetDepartment(int branId)
        {
            CompanyRepository dl = new CompanyRepository();
            var model = dl.DepartmentList(branId);
            return Json(new { Result = model.OrderBy(x => x.Name) });
        }
        #endregion
    }
}
