using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationServices.Web;
using DAL.Transaction;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Areas.SCM.Models.Master;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Report;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    [HandleError]
    
    public class NBOController : Controller
    {
        static List<NBOModel> exportList = new List<NBOModel>();

        static List<NBOModel> staffActivityDataCorreent = new List<NBOModel>();
        static List<NBOModel> staffActivityDataPrivious = new List<NBOModel>();

        //
        // GET: /NBO/

        //[ActionAuthorize]
        public ActionResult Index()
        {
            EmployeeRepository e = new EmployeeRepository();
            CompanyRepository c = new CompanyRepository();
            if (User.IsInRole("User"))
            {
                ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.AppLogin.ToLower() == User.Identity.Name.ToLower()), "Id", "EmpName");
                ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    int deptId = e.GetManagerDept(User.Identity.Name.ToUpper());
                    ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.Department.Id == deptId), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
                }
                else
                {
                    ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList(), "Id", "Name");
                }

            }

            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().Where(x => x.Id != 6).OrderBy(x => x.Name), "Id", "Name");

            EnquiryStatusRepository s = new EnquiryStatusRepository();
            ViewBag.Status = new SelectList(s.GetAll().OrderBy(x => x.Name), "Id", "Name");

            EnquirySourceRepository enq = new EnquirySourceRepository();
            ViewBag.source = new SelectList(enq.GetAll().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        public ActionResult Finance(int id, string file)
        {
            ViewBag.nboid = id;
            ViewBag.filenumber = file;
            return PartialView();
        }

        //Contracted / Operated / Closed file numbers.
        public ActionResult GetFileNumber(int branch)
        {
            try
            {
                NBODAL dal = new NBODAL();
                var list = from m in dal.GetAll().Where(x => x.Branch.Id == branch).OrderBy(x => x.FileNumber)
                           .Where(x => x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).ToList()
                           select new { Id = m.Id, Name = m.FileNumber };
                return Json(list, "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #region ---- NBO -----

        [HttpPost]
        public JsonResult List(int source = 0, string status = null, string col = null, int branch = 0, int empId = 0, int nature = 0, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {

            try
            {
                EmployeeRepository e = new EmployeeRepository();
                NBORepository dal = new NBORepository();
                List<NBOModel> model = new List<NBOModel>();
                if (User.IsInRole("User"))
                {
                    int emid = e.GetByName(User.Identity.Name.ToUpper());
                    model = dal.GetByEmployee(emid).ToList();
                    if (col == "top")
                    {
                        model = model.OrderByDescending(x => x.Id).Take(15).ToList();
                    }
                }
                else
                {
                    if (User.IsInRole("Manager"))
                    {
                        model = dal.GetByManager(User.Identity.Name.ToUpper()).ToList();
                        if (col == "top")
                        {
                            model = model.OrderByDescending(x => x.Id).Take(15).ToList();
                        }
                    }
                    else
                    {
                        if (col == "top")
                        {
                            model = dal.GetTop().OrderByDescending(x => x.Id).ToList();
                        }
                        else
                        {
                            if ((col == "File" || col != "File") && string.IsNullOrEmpty(name))
                                model = dal.GetAll().ToList();

                            if (col == "client" && !string.IsNullOrEmpty(name))
                                model = dal.GetAll().OrderByDescending(x => x.Id).ToList();

                            if (col == "event" && !string.IsNullOrEmpty(name))
                                model = dal.GetAll().OrderByDescending(x => x.Id).ToList();

                            if (col == "contact" && !string.IsNullOrEmpty(name))
                                model = dal.GetAll().OrderByDescending(x => x.Id).ToList();

                            if (col == "find" && !string.IsNullOrEmpty(name))
                                model = dal.GetAll().OrderByDescending(x => x.Id).ToList();

                            if (col == "source" && source != 0)
                                model = dal.GetAll().OrderByDescending(x => x.Id).ToList();
                        }


                    }
                }

                if (!string.IsNullOrEmpty(col))
                {
                    switch (col)
                    {
                        case "File":
                            if (!string.IsNullOrEmpty(name))
                            {
                                if (User.IsInRole("Finance") || User.IsInRole("Admin") || User.IsInRole("Management"))
                                {
                                    model = dal.GetByFile(name).ToList();
                                }
                                else
                                {
                                    model = model.Where(x => x.FileNumber == name).ToList();
                                }
                            }
                            else
                            {
                                model = model.Take(100).ToList();
                            }

                            break;
                        case "Emp":
                            if (empId != 0)
                                model = model.Where(x => x.EmpId == empId).ToList();
                            break;
                        case "branch":
                            if (branch != 0)
                                model = model.Where(x => x.Branch.Id == branch).ToList();
                            break;
                        case "st":
                            if (!String.IsNullOrEmpty(status))
                            {
                                string[] id = status.Split(',');
                                model = model.Where(x => id.Contains(x.Status.Id.ToString())).ToList();
                            }
                            break;
                        case "nature":
                            if (nature != 0)
                                model = model.Where(x => x.Nature.Id == nature).ToList();
                            break;
                        case "client":
                            model = model.Where(x => x.ClientName.Name.ToLower().StartsWith(name.ToLower())).ToList();
                            break;
                        case "event":
                            model = model.Where(x => !string.IsNullOrEmpty(x.EventName) && x.EventName.ToLower().StartsWith(name.ToLower())).ToList();
                            break;
                        case "contact":
                            model = model.Where(x => !string.IsNullOrEmpty(x.ContactName) && x.ContactName.ToLower().StartsWith(name.ToLower())).ToList();
                            break;
                        case "source":
                            model = model.Where(x => x.EnquirySource.Id == source).ToList();
                            break;
                        case "find":
                            model = model.Where(x =>
                                x.FileNumber.Contains(name) ||
                                x.ClientName.Name.StartsWith(name) ||
                                (x.EventName != null && x.EventName.ToLower().Contains(name.ToLower())) ||
                                (x.ContactName != null && x.ContactName.ToLower().Contains(name.ToLower())) ||
                                (x.EnquirySource.Name.ToLower().Contains(name.ToLower())) ||
                                (x.ClientCountry.Name.ToLower().Contains(name.ToLower())) ||
                                x.ContractValue.ToString().Contains(name) ||
                                x.ContractCost.ToString().Contains(name) ||
                                (x.EmailId != null && x.EmailId.ToLower().Contains(name.ToLower()))
                                ).ToList();
                            break;
                        case "filter":
                            model = dal.GetAllByFilter(model, status, branch, empId, nature, source).ToList();
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    model = model.OrderByDescending(x => x.RequestDate).Take(200).ToList();
                }

                exportList.Clear();
                exportList = model.ToList();
                model = model.OrderByDescending(x => x.Id).ToList();
                int count = model.Count;
                List<NBOModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new NBO
       
        [HttpPost]
        public JsonResult Create(NBOModel model)
        {
            model.RequestDate = Convert.ToDateTime(MyExtension.UAETime().ToShortDateString());
            model.StatusDate = Convert.ToDateTime(MyExtension.UAETime().ToShortDateString());
            if (model.ContractValue == null)
                model.ContractValue = 0;
            if (model.ContractCost == null)
                model.ContractCost = 0;
            try
            {
                if (NBORepository.ExistFile(model.FileNumber.Trim()) == true)
                {
                    return Json(new { Result = "ERROR", Message = "Filenumber already Exist!" });
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (Convert.ToDateTime(model.CheckinDate) > Convert.ToDateTime(model.CheckOutDate))
                {
                    return Json(new { Result = "ERROR", Message = "Check in date should not large than check out date!" });
                }
                NBORepository dal = new NBORepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update NBO details
       
        [HttpPost]
        public JsonResult Update(NBOModel model)
        {

            model.StatusDate = Convert.ToDateTime(MyExtension.UAETime().ToShortDateString());
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (model.StatusId != 1 && (model.ContractValue == 0 || model.ContractCost == 0))
                {
                    return Json(new { Result = "ERROR", Message = "Contract Value and Cost should not zero" });
                }
                if (Convert.ToDateTime(model.CheckinDate) > Convert.ToDateTime(model.CheckOutDate))
                {
                    return Json(new { Result = "ERROR", Message = "Check in date should not large than check out date!" });
                }
                if ((model.StatusId == 8 || model.StatusId == 9) && (model.Remarks == null || model.Remarks.Length < 10))
                {
                    return Json(new { Result = "ERROR", Message = "Minium remarks of 10 char" });
                }
                NBORepository dal = new NBORepository();
                dal.Edit(model);
                NBOModel Model = dal.GetById(model.Id);
                return Json(new { Result = "OK", Record = Model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
       
        public JsonResult Delete(NBOModel model)
        {
            try
            {
                NBORepository dal = new NBORepository();
                dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region -- NBO Comments ---

        //Create new NBO comments
        [HttpPost]
        public JsonResult AddComments(NBOCommentsModel model, int nboid)
        {
            model.Date = Convert.ToDateTime(MyExtension.UAETime().ToShortDateString());
            EmployeeRepository e = new EmployeeRepository();
            model.UserName = e.GetBy(User.Identity.Name.ToUpper()).EmpName;
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                NBOCommentsRepository dal = new NBOCommentsRepository();
                dal.AddComments(model, nboid);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Edit new NBO comments
        [HttpPost]
        public JsonResult EditComments(NBOCommentsModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                NBOCommentsRepository dal = new NBOCommentsRepository();
                dal.Edit(model);
                NBOCommentsModel data = dal.GetById(model.Id);
                return Json(new { Result = "OK", Record = data });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //List comments
        [HttpPost]
        public JsonResult ListComments(int nboid)
        {
            try
            {
                NBOCommentsRepository dal = new NBOCommentsRepository();
                var model = dal.GetNBOComments(nboid);
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
       
        public JsonResult DeleteComments(NBOCommentsModel model)
        {
            try
            {
                NBOCommentsRepository dal = new NBOCommentsRepository();
                dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region ---- Staff ----

        //[ActionAuthorize]
        public ActionResult StaffActivity()
        {
            EmployeeRepository e = new EmployeeRepository();
            CompanyRepository c = new CompanyRepository();

            if (User.IsInRole("User"))
            {
                ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.AppLogin.ToLower() == User.Identity.Name.ToLower()), "Id", "EmpName");
                ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    int deptid = e.GetManagerDept(User.Identity.Name.ToUpper());
                    ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.Department.Id == deptid), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
                }
                else
                {
                    ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList(), "Id", "Name");
                }
            }

            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        public ActionResult ActivityReport(string dateFrom = null, string dateTo = null, int branch = 0, int empId = 0, int natid = 0)
        {
            List<NBOModel> model = new List<NBOModel>();
            model = GetActivityReport(dateFrom, dateTo, branch, empId, natid).ToList();

            staffActivityDataCorreent.Clear();
            staffActivityDataCorreent = model.ToList();

            return PartialView(model);
        }

        public ActionResult ActivityReportPrivious(string dateFrom = null, string dateTo = null, int branch = 0, int empId = 0, int natid = 0)
        {
            string from = null;
            string to = null;
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                from = Convert.ToDateTime(dateFrom).AddYears(-1).ToShortDateString();
                to = Convert.ToDateTime(dateTo).AddYears(-1).ToShortDateString();
            }
            List<NBOModel> model = new List<NBOModel>();
            model = GetActivityReport(from, to, branch, empId, natid).ToList();

            staffActivityDataPrivious.Clear();
            staffActivityDataPrivious = model.ToList();

            return PartialView(model);
        }

        public List<NBOModel> GetActivityReport(string dateFrom = null, string dateTo = null, int branch = 0, int empId = 0, int natid = 0)
        {
            List<NBOModel> model = new List<NBOModel>();
            NBORepository dal = new NBORepository();
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            if (User.IsInRole("User"))
            {
                model = dal.GetAll().Where(x => (x.RequestDate >= Convert.ToDateTime(dateFrom) && x.RequestDate <= Convert.ToDateTime(dateTo))).ToList();
                model = dal.GetAllByFilter(model, null, branch, empId, natid).ToList();
            }
            if (User.IsInRole("Manager"))
            {
                model = dal.GetByManager(User.Identity.Name.ToUpper()).ToList();
                model = model.Where(x => (x.RequestDate >= Convert.ToDateTime(dateFrom) && x.RequestDate <= Convert.ToDateTime(dateTo))).ToList();
                model = dal.GetAllByFilter(model, null, branch, empId, natid).ToList();
            }
            else
            {
                model = dal.GetAll().Where(x => (x.RequestDate >= Convert.ToDateTime(dateFrom) && x.RequestDate <= Convert.ToDateTime(dateTo))).ToList();
                model = dal.GetAllByFilter(model, null, branch, empId, natid).ToList();
            }


            return model;
        }

        public ActionResult ExportStaffActivity(string dateFrom = null, string dateTo = null)
        {
            var CorrentData = staffActivityDataCorreent.ToList();
            var priviousData = staffActivityDataPrivious.ToList();

            #region -- Filtering data to fill table --

            var data1 = from n in CorrentData
                        group n by n.Nature.Name into g
                        select new
                        {
                            Nature = g.Key,
                            //ProposalStage
                            ProposalFile = g.Where(x => x.Status.Id == 1).Count(),
                            ProposalValue = g.Where(x => x.Status.Id == 1).Sum(x => x.ContractValue),
                            //Active/Potantial
                            ActiveFile = g.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Count(),
                            ActiveValue = g.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Sum(x => x.ContractValue),
                            //Confirmed/Contracted/Operated/Closed
                            ConfirmendFile = g.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Count(),
                            ConfirmendValue = g.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Sum(x => x.ContractValue),
                            //Cancelled /Inactive
                            CancelledFile = g.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Count(),
                            CancelledValue = g.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Sum(x => x.ContractValue),
                            TotalFiles = g.Count(),
                            TotalValue = g.Sum(x => x.ContractValue)
                        };

            var data2 = from n in priviousData
                        group n by n.Nature.Name into g
                        select new
                        {
                            Nature = g.Key,
                            //ProposalStage
                            ProposalFile = g.Where(x => x.Status.Id == 1).Count(),
                            ProposalValue = g.Where(x => x.Status.Id == 1).Sum(x => x.ContractValue),
                            //Active/Potantial
                            ActiveFile = g.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Count(),
                            ActiveValue = g.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Sum(x => x.ContractValue),
                            //Confirmed/Contracted/Operated/Closed
                            ConfirmendFile = g.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Count(),
                            ConfirmendValue = g.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Sum(x => x.ContractValue),
                            //Cancelled /Inactive
                            CancelledFile = g.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Count(),
                            CancelledValue = g.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Sum(x => x.ContractValue),
                            TotalFiles = g.Count(),
                            TotalValue = g.Sum(x => x.ContractValue)
                        };


            #endregion

            DataTable dt1 = new DataTable();

            #region -- Table Column Declarition --

            dt1.Columns.Add(new DataColumn("Nature", typeof(string)));
            dt1.Columns.Add(new DataColumn("ProposalFile", typeof(string)));
            dt1.Columns.Add(new DataColumn("ProposalValue", typeof(string)));
            dt1.Columns.Add(new DataColumn("Proposal%", typeof(string)));

            dt1.Columns.Add(new DataColumn("ActiveFile", typeof(string)));
            dt1.Columns.Add(new DataColumn("ActiveValue", typeof(string)));
            dt1.Columns.Add(new DataColumn("Active%", typeof(string)));

            dt1.Columns.Add(new DataColumn("ConfirmendFile", typeof(string)));
            dt1.Columns.Add(new DataColumn("ConfirmendValue", typeof(string)));
            dt1.Columns.Add(new DataColumn("Confirmend%", typeof(string)));

            dt1.Columns.Add(new DataColumn("CancelledFile", typeof(string)));
            dt1.Columns.Add(new DataColumn("CancelledValue", typeof(string)));
            dt1.Columns.Add(new DataColumn("Cancelled%", typeof(string)));

            dt1.Columns.Add(new DataColumn("TotalFiles", typeof(string)));
            dt1.Columns.Add(new DataColumn("TotalValue", typeof(string)));
            dt1.Columns.Add(new DataColumn("Total%", typeof(string)));

            #endregion

            #region -- Table internal Body --

            foreach (var item in data1.OrderBy(x => x.Nature))
            {
                double gTotal = Convert.ToDouble(data1.Sum(x => x.TotalFiles));
                double total = Convert.ToDouble(item.TotalFiles);
                double proposal = Convert.ToDouble(item.ProposalFile);
                double active = Convert.ToDouble(item.ActiveFile);
                double closed = Convert.ToDouble(item.ConfirmendFile);
                double Cancelled = Convert.ToDouble(item.CancelledFile);

                DataRow dr = dt1.NewRow();
                dr["Nature"] = item.Nature;

                dr["ProposalFile"] = item.ProposalFile;
                dr["ProposalValue"] = Convert.ToDouble(item.ProposalValue).ToString("##,##0");
                dr["Proposal%"] = (((proposal / total) * 100).ToString("##0.00"));

                dr["ActiveFile"] = item.ActiveFile;
                dr["ActiveValue"] = Convert.ToDouble(item.ActiveValue).ToString("##,##0");
                dr["Active%"] = (((active / total) * 100).ToString("##0.00"));

                dr["ConfirmendFile"] = item.ConfirmendFile;
                dr["ConfirmendValue"] = Convert.ToDouble(item.ConfirmendValue).ToString("##,##0");
                dr["Confirmend%"] = (((closed / total) * 100).ToString("##0.00"));

                dr["CancelledFile"] = item.CancelledFile;
                dr["CancelledValue"] = Convert.ToDouble(item.CancelledValue).ToString("##,##0");
                dr["Cancelled%"] = (((Cancelled / total) * 100).ToString("##0.00"));

                dr["TotalFiles"] = item.TotalFiles;
                dr["TotalValue"] = Convert.ToDouble(item.TotalValue).ToString("##,##0");
                dr["Total%"] = (((total / gTotal) * 100).ToString("##0"));

                dt1.Rows.Add(dr);
            }



            #endregion

            #region --- BPR table footer --

            double grandTotal = Convert.ToDouble(data1.Sum(x => x.TotalFiles));
            double grandproposal = Convert.ToDouble(data1.Sum(x => x.ProposalFile));
            double grandactive = Convert.ToDouble(data1.Sum(x => x.ActiveFile));
            double grandclosed = Convert.ToDouble(data1.Sum(x => x.ConfirmendFile));
            double grandCancelled = Convert.ToDouble(data1.Sum(x => x.CancelledFile));
            double totalFiles = Convert.ToDouble(data1.Sum(x => x.TotalFiles));

            DataRow f = dt1.NewRow();
            f["Nature"] = "Total";

            f["ProposalFile"] = data1.Sum(x => x.ProposalFile);
            f["ProposalValue"] = Convert.ToDouble(data1.Sum(x => x.ProposalValue)).ToString("##,##0");
            f["Proposal%"] = (((grandproposal / grandTotal) * 100).ToString("##0.00"));

            f["ActiveFile"] = data1.Sum(x => x.ActiveFile);
            f["ActiveValue"] = Convert.ToDouble(data1.Sum(x => x.ActiveValue)).ToString("##,##0");
            f["Active%"] = (((grandactive / grandTotal) * 100).ToString("##0.00"));

            f["ConfirmendFile"] = data1.Sum(x => x.ConfirmendFile);
            f["ConfirmendValue"] = Convert.ToDouble(data1.Sum(x => x.ConfirmendValue)).ToString("##,##0");
            f["Confirmend%"] = (((grandclosed / grandTotal) * 100).ToString("##0.00"));

            f["CancelledFile"] = data1.Sum(x => x.CancelledFile);
            f["CancelledValue"] = Convert.ToDouble(data1.Sum(x => x.CancelledValue)).ToString("##,##0");
            f["Cancelled%"] = (((grandCancelled / grandTotal) * 100).ToString("##0.00"));

            f["TotalFiles"] = totalFiles;
            f["TotalValue"] = Convert.ToDouble(data1.Sum(x => x.TotalValue)).ToString("##,##0");
            f["Total%"] = (((grandTotal / grandTotal) * 100).ToString("##0.00"));

            dt1.Rows.Add(f);

            #region -- Materialization ----

            DateTime dt = Convert.ToDateTime(dateFrom);
            DateTime to = Convert.ToDateTime(dateTo);
            double days = 0;
            double avgPerday = 0;

            if (dt != null && to != null)
            {
                days = (int)(to - dt).TotalDays;
                avgPerday = days == 0 ? 0 : (totalFiles / days);
            }

            DataRow blank = dt1.NewRow();
            dt1.Rows.Add(blank);

            DataRow r1 = dt1.NewRow();
            r1["Nature"] = "Average Per Day";
            r1["ProposalFile"] = avgPerday.ToString("0.00000");
            dt1.Rows.Add(r1);

            DataRow r2 = dt1.NewRow();
            r2["Nature"] = "Total Proposal & Active";
            r2["ProposalFile"] = (grandproposal + grandactive);
            dt1.Rows.Add(r2);

            DataRow r3 = dt1.NewRow();
            r3["Nature"] = "Materialization %";
            r3["ProposalFile"] = (((grandclosed / grandTotal) * 100).ToString("##.00"));
            dt1.Rows.Add(r3);

            #endregion


            #region -- Privious Year Data ---

            DataRow blank1 = dt1.NewRow();
            dt1.Rows.Add(blank1);

            DataRow p1 = dt1.NewRow();
            p1["Nature"] = "Privious Year Data";
            dt1.Rows.Add(p1);

            #region -- Table internal Body Privious Year --

            foreach (var item in data2.OrderBy(x => x.Nature))
            {
                double gTotal = Convert.ToDouble(data2.Sum(x => x.TotalFiles));
                double total = Convert.ToDouble(item.TotalFiles);
                double proposal = Convert.ToDouble(item.ProposalFile);
                double active = Convert.ToDouble(item.ActiveFile);
                double closed = Convert.ToDouble(item.ConfirmendFile);
                double Cancelled = Convert.ToDouble(item.CancelledFile);

                DataRow dr = dt1.NewRow();
                dr["Nature"] = item.Nature;

                dr["ProposalFile"] = item.ProposalFile;
                dr["ProposalValue"] = Convert.ToDouble(item.ProposalValue).ToString("##,##0");
                dr["Proposal%"] = (((proposal / total) * 100).ToString("##0.00"));

                dr["ActiveFile"] = item.ActiveFile;
                dr["ActiveValue"] = Convert.ToDouble(item.ActiveValue).ToString("##,##0");
                dr["Active%"] = (((active / total) * 100).ToString("##0.00"));

                dr["ConfirmendFile"] = item.ConfirmendFile;
                dr["ConfirmendValue"] = Convert.ToDouble(item.ConfirmendValue).ToString("##,##0");
                dr["Confirmend%"] = (((closed / total) * 100).ToString("##0.00"));

                dr["CancelledFile"] = item.CancelledFile;
                dr["CancelledValue"] = Convert.ToDouble(item.CancelledValue).ToString("##,##0");
                dr["Cancelled%"] = (((Cancelled / total) * 100).ToString("##0.00"));

                dr["TotalFiles"] = item.TotalFiles;
                dr["TotalValue"] = Convert.ToDouble(item.TotalValue).ToString("##,##0");
                dr["Total%"] = (((total / gTotal) * 100).ToString("##0"));

                dt1.Rows.Add(dr);
            }



            #endregion

            #region -- Table Footer ---

            double grandTotal2 = Convert.ToDouble(data2.Sum(x => x.TotalFiles));
            double grandproposal2 = Convert.ToDouble(data2.Sum(x => x.ProposalFile));
            double grandactive2 = Convert.ToDouble(data2.Sum(x => x.ActiveFile));
            double grandclosed2 = Convert.ToDouble(data2.Sum(x => x.ConfirmendFile));
            double grandCancelled2 = Convert.ToDouble(data2.Sum(x => x.CancelledFile));
            double totalFiles2 = Convert.ToDouble(data2.Sum(x => x.TotalFiles));

            DataRow p = dt1.NewRow();
            p["Nature"] = "Total";

            p["ProposalFile"] = data2.Sum(x => x.ProposalFile);
            p["ProposalValue"] = Convert.ToDouble(data2.Sum(x => x.ProposalValue)).ToString("##,##0");
            p["Proposal%"] = (((grandproposal2 / grandTotal2) * 100).ToString("##0.00"));

            p["ActiveFile"] = data2.Sum(x => x.ActiveFile);
            p["ActiveValue"] = Convert.ToDouble(data2.Sum(x => x.ActiveValue)).ToString("##,##0");
            p["Active%"] = (((grandactive2 / grandTotal2) * 100).ToString("##0.00"));

            p["ConfirmendFile"] = data2.Sum(x => x.ConfirmendFile);
            p["ConfirmendValue"] = Convert.ToDouble(data2.Sum(x => x.ConfirmendValue)).ToString("##,##0");
            p["Confirmend%"] = (((grandclosed2 / grandTotal2) * 100).ToString("##0.00"));

            p["CancelledFile"] = data2.Sum(x => x.CancelledFile);
            p["CancelledValue"] = Convert.ToDouble(data2.Sum(x => x.CancelledValue)).ToString("##,##0");
            p["Cancelled%"] = (((grandCancelled2 / grandTotal2) * 100).ToString("##0.00"));

            p["TotalFiles"] = totalFiles2;
            p["TotalValue"] = Convert.ToDouble(data2.Sum(x => x.TotalValue)).ToString("##,##0");
            p["Total%"] = (((grandTotal2 / grandTotal2) * 100).ToString("##0.00"));

            dt1.Rows.Add(p);

            #endregion

            #region -- Materialization ----

            DateTime dt2 = Convert.ToDateTime(dateFrom).AddYears(-1);
            DateTime to2 = Convert.ToDateTime(dateTo).AddYears(-1);
            double days2 = 0;
            double avgPerday2 = 0;

            if (dt2 != null && to2 != null)
            {
                days2 = (int)(to2 - dt2).TotalDays;
                avgPerday2 = days2 == 0 ? 0 : (totalFiles2 / days2);
            }

            DataRow blank3 = dt1.NewRow();
            dt1.Rows.Add(blank3);

            DataRow m1 = dt1.NewRow();
            m1["Nature"] = "Average Per Day";
            m1["ProposalFile"] = avgPerday2.ToString("0.00000");
            dt1.Rows.Add(m1);

            DataRow m2 = dt1.NewRow();
            m2["Nature"] = "Total Proposal & Active";
            m2["ProposalFile"] = (grandproposal2 + grandactive2);
            dt1.Rows.Add(m2);

            DataRow m3 = dt1.NewRow();
            m3["Nature"] = "Materialization %";
            m3["ProposalFile"] = (((grandclosed2 / grandTotal2) * 100).ToString("##.00"));
            dt1.Rows.Add(m3);

            #endregion

            #endregion


            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt1;
            gv.DataBind();

            #region -- Table Top Header ---

            TableCell cell = new TableCell();
            cell.Text = "Nature";
            cell.HorizontalAlign = HorizontalAlign.Center;
            GridViewRow headerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            headerRow.Cells.Add(cell);

            TableCell cell2 = new TableCell();
            cell2.Text = "ProposalStage";
            cell2.ColumnSpan = 3;
            cell2.HorizontalAlign = HorizontalAlign.Center;
            cell2.BackColor = Color.Yellow;
            headerRow.Cells.Add(cell2);

            TableCell cell3 = new TableCell();
            cell3.Text = "Active/Potantial";
            cell3.ColumnSpan = 3;
            cell3.Wrap = true;
            cell3.HorizontalAlign = HorizontalAlign.Center;
            cell3.BackColor = Color.LightGreen;
            headerRow.Cells.Add(cell3);

            TableCell cell4 = new TableCell();
            cell4.Text = "Confirmed/Contracted/Operated/Closed";
            cell4.ColumnSpan = 3;
            cell4.HorizontalAlign = HorizontalAlign.Center;
            cell4.BackColor = Color.LightBlue;
            headerRow.Cells.Add(cell4);

            TableCell cell5 = new TableCell();
            cell5.Text = "Cancelled /Inactive";
            cell5.ColumnSpan = 3;
            cell5.HorizontalAlign = HorizontalAlign.Center;
            cell5.BackColor = Color.Red;
            headerRow.Cells.Add(cell5);

            TableCell cell6 = new TableCell();
            cell6.Text = "Total";
            cell6.ColumnSpan = 3;
            cell6.HorizontalAlign = HorizontalAlign.Center;
            cell6.BackColor = Color.Red;
            headerRow.Cells.Add(cell6);

            gv.Controls[0].Controls.AddAt(0, headerRow);

            GridViewRow lowerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell c1 = new TableCell();
            TableCell c2 = new TableCell();
            TableCell c3 = new TableCell();
            TableCell c4 = new TableCell();
            TableCell c5 = new TableCell();
            TableCell c6 = new TableCell();
            TableCell c7 = new TableCell();
            TableCell c8 = new TableCell();
            TableCell c9 = new TableCell();
            TableCell c10 = new TableCell();
            TableCell c11 = new TableCell();
            TableCell c12 = new TableCell();
            TableCell c13 = new TableCell();
            TableCell c14 = new TableCell();
            TableCell c15 = new TableCell();
            TableCell c16 = new TableCell();
            TableCell c17 = new TableCell();

            c1.Text = "Nature";
            //ProposalStage
            c2.Text = "No Of Files";
            c3.Text = "Value";
            c4.Text = "%";
            //Active
            c5.Text = "No Of Files";
            c6.Text = "Value";
            c7.Text = "%";
            //Confirmed
            c8.Text = "No Of Files";
            c9.Text = "Value";
            c10.Text = "%";
            //Cancelled
            c11.Text = "No Of Files";
            c12.Text = "Value";
            c13.Text = "%";
            //Total
            c14.Text = "No Of Files";
            c15.Text = "Value";
            c16.Text = "%";

            lowerRow.Cells.Add(c1);
            lowerRow.Cells.Add(c2);
            lowerRow.Cells.Add(c3);
            lowerRow.Cells.Add(c4);
            lowerRow.Cells.Add(c5);
            lowerRow.Cells.Add(c6);
            lowerRow.Cells.Add(c7);
            lowerRow.Cells.Add(c8);
            lowerRow.Cells.Add(c9);
            lowerRow.Cells.Add(c10);
            lowerRow.Cells.Add(c11);
            lowerRow.Cells.Add(c12);
            lowerRow.Cells.Add(c13);
            lowerRow.Cells.Add(c14);
            lowerRow.Cells.Add(c15);
            lowerRow.Cells.Add(c16);

            gv.Controls[0].Controls.RemoveAt(1);
            gv.Controls[0].Controls.AddAt(1, lowerRow);

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Date From:";
            cell_1.BackColor = Color.Yellow;
            cell_1.HorizontalAlign = HorizontalAlign.Left;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = dateFrom;
            cell_11.ColumnSpan = 15;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "Date To:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = dateTo;
            cell_22.ColumnSpan = 15;
            row2.Cells.Add(cell_22);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);

            #endregion

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=StaffActivity.xls");

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

        #endregion

        #region ---- Client ----

        //[ActionAuthorize]
        public ActionResult ClientAnalysisIndex()
        {
            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().OrderBy(x => x.Name), "Id", "Name");

            CompanyRepository c = new CompanyRepository();
            ViewBag.Branch = new SelectList(c.BranchList().OrderBy(x => x.Name), "Id", "Name");


            return View();
        }

        public AnayalisModel ClientData(string client = null, int branch = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            List<NBOModel> model = new List<NBOModel>();
            NBORepository dal = new NBORepository();

            AnayalisModel anayalis = new AnayalisModel();
            AnaylysisRepository rep = new AnaylysisRepository();

            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                model = dal.GetAll().ToList();

                if (branch != 0 && client == "")
                {
                    model = model.Where(x => x.Branch.Id == branch).ToList();
                }
                if (branch == 0 && client != "")
                {
                    string[] list = client.Split(',');
                    model = model.Where(x => list.Contains(x.ClientId.ToString())).ToList();
                }
                if (natid != 0 && client == "")
                {
                    if (natid == 6)
                    {
                        model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                    }
                    else
                    {
                        model = model.Where(x => x.Nature.Id == natid).ToList();
                    }

                }
                if (natid != 0 && client != "")
                {
                    if (natid == 6)
                    {
                        model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                    }
                    else
                    {
                        model = model.Where(x => x.Nature.Id == natid).ToList();
                    }
                    string[] list = client.Split(',');
                    model = model.Where(x => list.Contains(x.ClientId.ToString())).ToList();
                }

                if (branch != 0 && client != "")
                {
                    model = model.Where(x => x.Branch.Id == branch).ToList();
                    string[] list = client.Split(',');
                    model = model.Where(x => list.Contains(x.ClientId.ToString())).ToList();
                }
                if (branch != 0 && client == "")
                {
                    model = model.Where(x => x.Branch.Id == branch).ToList();
                }

            }
            else
            {
                model = dal.GetAll().Take(0).ToList();
            }

            anayalis.CurrentNBO = rep.GetCurrentNBO(model, dateFrom, dateTo);
            anayalis.PriviousNBO = rep.GetPriviousNBO(model, dateFrom, dateTo);

            return anayalis;

        }

        public ActionResult ClientAnalysis(int branch = 0, string client = null, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            AnayalisModel anayalis = new AnayalisModel();
            anayalis = ClientData(client, branch, dateFrom, dateTo, natid);
            return PartialView(anayalis);

            //return PartialView(model);
        }

        public ActionResult ExportClient(string client = null, int branch = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            AnayalisModel model = new AnayalisModel();
            model = ClientData(client, branch, dateFrom, dateTo, natid);

            #region -- Filtering data to fill table --

            var currentC = from c in model.CurrentNBO
                           select new { Client = c.ClientName.Name, Country = c.ClientName.Country.Name };
            var priviousC = from c in model.PriviousNBO
                            select new { Client = c.ClientName.Name, Country = c.ClientName.Country.Name };

            var cont = currentC.Concat(priviousC).Distinct();

            var data = from n in model.CurrentNBO
                       group n by n.ClientName into s
                       select new
                       {
                           Client = s.Key.Name,
                           Country = s.Key.Country.Name,
                           //Active/Potantial
                           ActiveValue = s.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Sum(x => x.ContractValue),
                           ActiveFiles = s.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Count(),
                           //Cancelled /Inactive
                           CancelledValue = s.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Sum(x => x.ContractValue),
                           CancelledFiles = s.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Count(),
                           //Confirmed/Contracted/Operated/Closed
                           ConfirmendValue = s.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Sum(x => x.ContractValue),
                           ConfirmendFiles = s.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Count(),
                           //ProposalStage
                           ProposalStage = s.Where(x => x.Status.Id == 1).Sum(x => x.ContractValue),
                           ProposalFiles = s.Where(x => x.Status.Id == 1).Count(),
                           TotalFiles = s.Count(),
                           TotalValue = s.Sum(x => x.ContractValue),
                       };

            var privious = from n in model.PriviousNBO
                           group n by n.ClientName into s
                           select new
                           {
                               Client = s.Key.Name,
                               Country = s.Key.Country.Name,
                               TotalFiles = s.Count(),
                               TotalValue = s.Sum(x => x.ContractValue),
                           };

            var query = from c in cont
                        join m in data on c.Client equals m.Client into mb
                        join b in privious on c.Client equals b.Client into pv
                        from current in mb.DefaultIfEmpty()
                        from pri in pv.DefaultIfEmpty()
                        select new
                        {
                            Client = c.Client,
                            Country = c.Country,
                            ActiveValue = current == null ? 0 : current.ActiveValue,
                            ActiveFiles = current == null ? 0 : current.ActiveFiles,
                            CancelledValue = current == null ? 0 : current.CancelledValue,
                            CancelledFiles = current == null ? 0 : current.CancelledFiles,
                            ConfirmendValue = current == null ? 0 : current.ConfirmendValue,
                            ConfirmendFiles = current == null ? 0 : current.ConfirmendFiles,
                            ProposalStage = current == null ? 0 : current.ProposalStage,
                            ProposalFiles = current == null ? 0 : current.ProposalFiles,
                            TotalFiles = current == null ? 0 : current.TotalFiles,
                            TotalValue = current == null ? 0 : current.TotalValue,
                            LastFiles = (pri == null ? 0 : pri.TotalFiles),
                            LastValues = (pri == null ? 0 : pri.TotalValue)
                        };

            #endregion

            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("Client", typeof(string)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));

            dt.Columns.Add(new DataColumn("Pro-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Pro-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Act-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Act-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Con-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Con-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Can-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Can-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Total-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Total-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Last-TotalFiles", typeof(string)));
            dt.Columns.Add(new DataColumn("Last-TotalValues", typeof(string)));

            #endregion

            #region -- Table internal Body --

            foreach (var item in query.OrderBy(x => x.Client))
            {
                DataRow dr = dt.NewRow();
                dr["Client"] = item.Client;
                dr["Country"] = item.Country;

                dr["Pro-Files"] = item.ProposalFiles;
                dr["Pro-Values"] = Convert.ToDouble(item.ProposalStage).ToString("##,##0");

                dr["Act-Files"] = item.ActiveFiles;
                dr["Act-Values"] = Convert.ToDouble(item.ActiveValue).ToString("##,##0");

                dr["Con-Files"] = item.ConfirmendFiles;
                dr["Con-Values"] = Convert.ToDouble(item.ConfirmendValue).ToString("##,##0");

                dr["Can-Files"] = item.CancelledFiles;
                dr["Can-Values"] = Convert.ToDouble(item.CancelledValue).ToString("##,##0");

                dr["Total-Files"] = item.TotalFiles;
                dr["Total-Values"] = Convert.ToDouble(item.TotalValue).ToString("##,##0");

                dr["Last-TotalFiles"] = item.LastFiles;
                dr["Last-TotalValues"] = Convert.ToDouble(item.LastValues).ToString("##,##0");

                dt.Rows.Add(dr);
            }

            #endregion

            #region --- BPR table footer --

            DataRow f = dt.NewRow();
            f["Client"] = "Total";

            f["Pro-Files"] = query.Sum(x => x.ProposalFiles);
            f["Pro-Values"] = Convert.ToDouble(query.Sum(x => x.ProposalStage)).ToString("##,##0");

            f["Act-Files"] = query.Sum(x => x.ActiveFiles);
            f["Act-Values"] = Convert.ToDouble(query.Sum(x => x.ActiveValue)).ToString("##,##0");

            f["Con-Files"] = query.Sum(x => x.ConfirmendFiles);
            f["Con-Values"] = Convert.ToDouble(query.Sum(x => x.ConfirmendValue)).ToString("##,##0");

            f["Can-Files"] = query.Sum(x => x.CancelledFiles);
            f["Can-Values"] = Convert.ToDouble(query.Sum(x => x.CancelledValue)).ToString("##,##0");

            f["Total-Files"] = query.Sum(x => x.TotalFiles);
            f["Total-Values"] = Convert.ToDouble(query.Sum(x => x.TotalValue)).ToString("##,##0");

            f["Last-TotalFiles"] = query.Sum(x => x.LastFiles);
            f["Last-TotalValues"] = Convert.ToDouble(query.Sum(x => x.LastValues)).ToString("##,##0");

            dt.Rows.Add(f);

            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            #region -- Table Top Header ---

            GridViewRow headerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell1 = new TableCell();
            cell1.Text = "Client";
            cell1.RowSpan = 2;
            cell1.HorizontalAlign = HorizontalAlign.Center;
            cell1.VerticalAlign = VerticalAlign.Middle;
            cell1.BackColor = Color.Silver;
            headerRow.Cells.Add(cell1);

            TableCell cell11 = new TableCell();
            cell11.Text = "Country";
            cell11.RowSpan = 2;
            cell11.HorizontalAlign = HorizontalAlign.Center;
            cell11.VerticalAlign = VerticalAlign.Middle;
            cell11.BackColor = Color.Silver;
            headerRow.Cells.Add(cell11);

            TableCell cell2 = new TableCell();
            cell2.Text = "Proposal Stage";
            cell2.ColumnSpan = 2;
            cell2.HorizontalAlign = HorizontalAlign.Center;
            cell2.BackColor = Color.Yellow;
            headerRow.Cells.Add(cell2);

            TableCell cell3 = new TableCell();
            cell3.Text = "Active/Potantial";
            cell3.ColumnSpan = 2;
            cell3.HorizontalAlign = HorizontalAlign.Center;
            cell3.BackColor = Color.LightGreen;
            headerRow.Cells.Add(cell3);

            TableCell cell4 = new TableCell();
            cell4.Text = "Confirmed / Contracted / Operated / Closed";
            cell4.ColumnSpan = 2;
            cell4.Wrap = true;
            cell4.HorizontalAlign = HorizontalAlign.Center;
            cell4.BackColor = Color.LightBlue;
            headerRow.Cells.Add(cell4);

            TableCell cell5 = new TableCell();
            cell5.Text = "Cancelled /InActive";
            cell5.ColumnSpan = 2;
            cell5.HorizontalAlign = HorizontalAlign.Center;
            cell5.BackColor = Color.Red;
            headerRow.Cells.Add(cell5);

            TableCell cell6 = new TableCell();
            cell6.Text = "Total";
            cell6.ColumnSpan = 2;
            cell6.HorizontalAlign = HorizontalAlign.Center;
            cell6.BackColor = Color.Violet;
            headerRow.Cells.Add(cell6);

            TableCell cell7 = new TableCell();
            cell7.Text = "Last Year Total";
            cell7.ColumnSpan = 2;
            cell7.HorizontalAlign = HorizontalAlign.Center;
            cell7.BackColor = Color.Blue;
            headerRow.Cells.Add(cell7);
            gv.Controls[0].Controls.AddAt(0, headerRow);

            GridViewRow lowerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell c1 = new TableCell();
            TableCell c2 = new TableCell();
            TableCell c3 = new TableCell();
            TableCell c4 = new TableCell();
            TableCell c5 = new TableCell();
            TableCell c6 = new TableCell();
            TableCell c7 = new TableCell();
            TableCell c8 = new TableCell();
            TableCell c9 = new TableCell();
            TableCell c10 = new TableCell();
            TableCell c11 = new TableCell();
            TableCell c12 = new TableCell();

            c1.BackColor = Color.Silver;
            c2.BackColor = Color.Silver;
            c3.BackColor = Color.Silver;
            c4.BackColor = Color.Silver;
            c5.BackColor = Color.Silver;
            c6.BackColor = Color.Silver;
            c7.BackColor = Color.Silver;
            c8.BackColor = Color.Silver;
            c9.BackColor = Color.Silver;
            c10.BackColor = Color.Silver;
            c11.BackColor = Color.Silver;
            c12.BackColor = Color.Silver;

            c1.Text = "No";
            c1.Width = new Unit(50);
            c2.Text = "Value";
            c2.Width = new Unit(80);
            c3.Text = "No";
            c3.Width = new Unit(50);
            c4.Text = "Value";
            c4.Width = new Unit(80);
            c5.Text = "No";
            c5.Width = new Unit(50);
            c6.Text = "Value";
            c6.Width = new Unit(80);
            c7.Text = "No";
            c7.Width = new Unit(50);
            c8.Text = "Value";
            c8.Width = new Unit(80);
            c9.Text = "No";
            c9.Width = new Unit(50);
            c10.Text = "Value";
            c10.Width = new Unit(80);
            c11.Text = "No";
            c11.Width = new Unit(50);
            c12.Text = "Value";
            c12.Width = new Unit(80);

            lowerRow.Cells.Add(c1);
            lowerRow.Cells.Add(c2);
            lowerRow.Cells.Add(c3);
            lowerRow.Cells.Add(c4);
            lowerRow.Cells.Add(c5);
            lowerRow.Cells.Add(c6);
            lowerRow.Cells.Add(c7);
            lowerRow.Cells.Add(c8);
            lowerRow.Cells.Add(c9);
            lowerRow.Cells.Add(c10);
            lowerRow.Cells.Add(c11);
            lowerRow.Cells.Add(c12);

            gv.Controls[0].Controls.RemoveAt(1);
            gv.Controls[0].Controls.AddAt(1, lowerRow);

            #endregion

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row3 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row4 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row5 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Branch:";
            cell_1.BackColor = Color.Yellow;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = CompanyRepository.BranchName(branch);
            cell_11.ColumnSpan = 13;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "File Handler:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = "All";
            cell_22.ColumnSpan = 13;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "Nature:";
            cell_3.BackColor = Color.Yellow;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = BusinessNatureRepository.NatureName(natid);
            cell_33.ColumnSpan = 13;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Request Date:";
            cell_4.BackColor = Color.Yellow;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = "From:" + Convert.ToDateTime(dateFrom).ToString("dd MMM yyyy") + " To: " + Convert.ToDateTime(dateTo).ToString("dd MMM yyyy");
            cell_44.ColumnSpan = 13;
            row4.Cells.Add(cell_44);

            TableCell cell_5 = new TableCell();
            cell_5.Text = "Client:";
            cell_5.BackColor = Color.Yellow;
            row5.Cells.Add(cell_5);

            TableCell cell_55 = new TableCell();
            cell_55.Text = ClientRepository.ClientName(client);
            cell_55.ColumnSpan = 13;
            row5.Cells.Add(cell_55);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);
            gv.Controls[0].Controls.AddAt(4, row5);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ClientAnalysis.xls");

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

        #endregion

        #region ---- Market ----

        //[ActionAuthorize]
        public ActionResult MarketAnalysisIndex()
        {
            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().OrderBy(x => x.Name), "Id", "Name");

            CompanyRepository c = new CompanyRepository();
            ViewBag.Branch = new SelectList(c.BranchList().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        public AnayalisModel MarketData(string country = null, int branch = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            List<NBOModel> model = new List<NBOModel>();
            NBORepository dal = new NBORepository();

            AnayalisModel anayalis = new AnayalisModel();
            AnaylysisRepository rep = new AnaylysisRepository();

            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                model = dal.GetAll().ToList();

                if (branch != 0 && country == "")
                {
                    model = model.Where(x => x.Branch.Id == branch).ToList();
                }
                if (branch == 0 && country != "")
                {
                    string[] list = country.Split(',');
                    model = model.Where(x => list.Contains(x.CountryId.ToString())).ToList();
                }
                if (natid != 0 && country == "")
                {
                    if (natid == 6)
                    {
                        model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                    }
                    else
                    {
                        model = model.Where(x => x.Nature.Id == natid).ToList();
                    }
                }
                if (natid != 0 && country != "")
                {
                    if (natid == 6)
                    {
                        model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                    }
                    else
                    {
                        model = model.Where(x => x.Nature.Id == natid).ToList();
                    }
                    string[] list = country.Split(',');
                    model = model.Where(x => list.Contains(x.CountryId.ToString())).ToList();
                }
                if (branch != 0 && country != "")
                {
                    model = model.Where(x => x.Branch.Id == branch).ToList();
                    string[] list = country.Split(',');
                    model = model.Where(x => list.Contains(x.CountryId.ToString())).ToList();
                }
                if (branch != 0 && country == "")
                {
                    model = model.Where(x => x.Branch.Id == branch).ToList();
                }
            }
            else
            {
                model = dal.GetAll().Take(0).ToList();
            }

            anayalis.CurrentNBO = rep.GetCurrentNBO(model, dateFrom, dateTo);
            anayalis.PriviousNBO = rep.GetPriviousNBO(model, dateFrom, dateTo);

            return anayalis;

        }

        public ActionResult MarketAnalysis(string country = null, int branch = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            AnayalisModel anayalis = new AnayalisModel();
            anayalis = MarketData(country, branch, dateFrom, dateTo, natid);
            return PartialView(anayalis);
            //return PartialView(model);
        }

        public ActionResult ExportMarket(string country = null, int branch = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            AnayalisModel model = new AnayalisModel();
            model = MarketData(country, branch, dateFrom, dateTo, natid);

            #region -- Filtering data to fill table --

            var currentC = from c in model.CurrentNBO
                           select new { Country = c.ClientCountry.Name };
            var priviousC = from c in model.PriviousNBO
                            select new { Country = c.ClientCountry.Name };

            var cont = currentC.Concat(priviousC).Distinct();

            var details = from n in model.CurrentNBO
                          group n by n.ClientCountry.Name into s
                          select new
                          {
                              Country = s.Key,
                              //Active/Potantial
                              ActiveValue = s.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Sum(x => x.ContractValue),
                              ActiveFiles = s.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Count(),
                              //Cancelled /Inactive
                              CancelledValue = s.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Sum(x => x.ContractValue),
                              CancelledFiles = s.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Count(),
                              //Confirmed/Contracted/Operated/Closed
                              ConfirmendValue = s.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Sum(x => x.ContractValue),
                              ConfirmendFiles = s.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Count(),
                              //ProposalStage
                              ProposalStage = s.Where(x => x.Status.Id == 1).Sum(x => x.ContractValue),
                              ProposalFiles = s.Where(x => x.Status.Id == 1).Count(),
                              TotalFiles = s.Count(),
                              TotalValue = s.Sum(x => x.ContractValue),
                          };

            var privious = from n in model.PriviousNBO
                           group n by n.ClientCountry.Name into s
                           select new
                           {
                               Country = s.Key,
                               TotalFiles = s.Count(),
                               TotalValue = s.Sum(x => x.ContractValue),
                           };

            var query = from c in cont
                        join m in details on c.Country equals m.Country into mb
                        join b in privious on c.Country equals b.Country into pv
                        from current in mb.DefaultIfEmpty()
                        from pri in pv.DefaultIfEmpty()
                        select new
                        {
                            Country = c.Country,
                            ActiveValue = current == null ? 0 : current.ActiveValue,
                            ActiveFiles = current == null ? 0 : current.ActiveFiles,
                            CancelledValue = current == null ? 0 : current.CancelledValue,
                            CancelledFiles = current == null ? 0 : current.CancelledFiles,
                            ConfirmendValue = current == null ? 0 : current.ConfirmendValue,
                            ConfirmendFiles = current == null ? 0 : current.ConfirmendFiles,
                            ProposalStage = current == null ? 0 : current.ProposalStage,
                            ProposalFiles = current == null ? 0 : current.ProposalFiles,
                            TotalFiles = current == null ? 0 : current.TotalFiles,
                            TotalValue = current == null ? 0 : current.TotalValue,
                            LastFiles = (pri == null ? 0 : pri.TotalFiles),
                            LastValues = (pri == null ? 0 : pri.TotalValue)
                        };

            #endregion

            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("Country", typeof(string)));

            dt.Columns.Add(new DataColumn("Pro-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Pro-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Act-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Act-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Con-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Con-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Can-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Can-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Total-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Total-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Last-TotalFiles", typeof(string)));
            dt.Columns.Add(new DataColumn("Last-TotalValues", typeof(string)));

            #endregion

            #region -- Table internal Body --

            foreach (var item in query.OrderBy(x => x.Country))
            {
                DataRow dr = dt.NewRow();
                dr["Country"] = item.Country;

                dr["Pro-Files"] = item.ProposalFiles;
                dr["Pro-Values"] = Convert.ToDouble(item.ProposalStage).ToString("##,##0");

                dr["Act-Files"] = item.ActiveFiles;
                dr["Act-Values"] = Convert.ToDouble(item.ActiveValue).ToString("##,##0");

                dr["Con-Files"] = item.ConfirmendFiles;
                dr["Con-Values"] = Convert.ToDouble(item.ConfirmendValue).ToString("##,##0");

                dr["Can-Files"] = item.CancelledFiles;
                dr["Can-Values"] = Convert.ToDouble(item.CancelledValue).ToString("##,##0");

                dr["Total-Files"] = item.TotalFiles;
                dr["Total-Values"] = Convert.ToDouble(item.TotalValue).ToString("##,##0");

                dr["Last-TotalFiles"] = item.LastFiles;
                dr["Last-TotalValues"] = Convert.ToDouble(item.LastValues).ToString("##,##0");

                dt.Rows.Add(dr);
            }

            #endregion

            #region --- BPR table footer --

            DataRow f = dt.NewRow();
            f["Country"] = "Total";

            f["Pro-Files"] = query.Sum(x => x.ProposalFiles);
            f["Pro-Values"] = Convert.ToDouble(query.Sum(x => x.ProposalStage)).ToString("##,##0");

            f["Act-Files"] = query.Sum(x => x.ActiveFiles);
            f["Act-Values"] = Convert.ToDouble(query.Sum(x => x.ActiveValue)).ToString("##,##0");

            f["Con-Files"] = query.Sum(x => x.ConfirmendFiles);
            f["Con-Values"] = Convert.ToDouble(query.Sum(x => x.ConfirmendValue)).ToString("##,##0");

            f["Can-Files"] = query.Sum(x => x.CancelledFiles);
            f["Can-Values"] = Convert.ToDouble(query.Sum(x => x.CancelledValue)).ToString("##,##0");

            f["Total-Files"] = query.Sum(x => x.TotalFiles);
            f["Total-Values"] = Convert.ToDouble(query.Sum(x => x.TotalValue)).ToString("##,##0");

            f["Last-TotalFiles"] = query.Sum(x => x.LastFiles);
            f["Last-TotalValues"] = Convert.ToDouble(query.Sum(x => x.LastValues)).ToString("##,##0");

            dt.Rows.Add(f);

            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            #region -- Table Top Header ---

            GridViewRow headerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell = new TableCell();
            cell.Text = "Country";
            cell.RowSpan = 2;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.BackColor = Color.Silver;
            headerRow.Cells.Add(cell);

            TableCell cell2 = new TableCell();
            cell2.Text = "Proposal Stage";
            cell2.ColumnSpan = 2;
            cell2.HorizontalAlign = HorizontalAlign.Center;
            cell2.BackColor = Color.Yellow;
            headerRow.Cells.Add(cell2);

            TableCell cell3 = new TableCell();
            cell3.Text = "Active/Potantial";
            cell3.ColumnSpan = 2;
            cell3.HorizontalAlign = HorizontalAlign.Center;
            cell3.BackColor = Color.LightGreen;
            headerRow.Cells.Add(cell3);

            TableCell cell4 = new TableCell();
            cell4.Text = "Confirmed / Contracted / Operated / Closed";
            cell4.ColumnSpan = 2;
            cell4.Wrap = true;
            cell4.HorizontalAlign = HorizontalAlign.Center;
            cell4.BackColor = Color.LightBlue;
            headerRow.Cells.Add(cell4);

            TableCell cell5 = new TableCell();
            cell5.Text = "Cancelled /InActive";
            cell5.ColumnSpan = 2;
            cell5.HorizontalAlign = HorizontalAlign.Center;
            cell5.BackColor = Color.Red;
            headerRow.Cells.Add(cell5);

            TableCell cell6 = new TableCell();
            cell6.Text = "Total";
            cell6.ColumnSpan = 2;
            cell6.HorizontalAlign = HorizontalAlign.Center;
            cell6.BackColor = Color.Violet;
            headerRow.Cells.Add(cell6);

            TableCell cell7 = new TableCell();
            cell7.Text = "Last Year Total";
            cell7.ColumnSpan = 2;
            cell7.HorizontalAlign = HorizontalAlign.Center;
            cell7.BackColor = Color.Blue;
            headerRow.Cells.Add(cell7);

            gv.Controls[0].Controls.AddAt(0, headerRow);

            GridViewRow lowerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell c1 = new TableCell();
            TableCell c2 = new TableCell();
            TableCell c3 = new TableCell();
            TableCell c4 = new TableCell();
            TableCell c5 = new TableCell();
            TableCell c6 = new TableCell();
            TableCell c7 = new TableCell();
            TableCell c8 = new TableCell();
            TableCell c9 = new TableCell();
            TableCell c10 = new TableCell();
            TableCell c11 = new TableCell();
            TableCell c12 = new TableCell();

            c1.BackColor = Color.Silver;
            c2.BackColor = Color.Silver;
            c3.BackColor = Color.Silver;
            c4.BackColor = Color.Silver;
            c5.BackColor = Color.Silver;
            c6.BackColor = Color.Silver;
            c7.BackColor = Color.Silver;
            c8.BackColor = Color.Silver;
            c9.BackColor = Color.Silver;
            c10.BackColor = Color.Silver;
            c11.BackColor = Color.Silver;
            c12.BackColor = Color.Silver;

            c1.Text = "No";
            c1.Width = new Unit(50);
            c2.Text = "Value";
            c2.Width = new Unit(80);
            c3.Text = "No";
            c3.Width = new Unit(50);
            c4.Text = "Value";
            c4.Width = new Unit(80);
            c5.Text = "No";
            c5.Width = new Unit(50);
            c6.Text = "Value";
            c6.Width = new Unit(80);
            c7.Text = "No";
            c7.Width = new Unit(50);
            c8.Text = "Value";
            c8.Width = new Unit(80);
            c9.Text = "No";
            c9.Width = new Unit(50);
            c10.Text = "Value";
            c10.Width = new Unit(80);
            c11.Text = "No";
            c11.Width = new Unit(50);
            c12.Text = "Value";
            c12.Width = new Unit(80);

            lowerRow.Cells.Add(c1);
            lowerRow.Cells.Add(c2);
            lowerRow.Cells.Add(c3);
            lowerRow.Cells.Add(c4);
            lowerRow.Cells.Add(c5);
            lowerRow.Cells.Add(c6);
            lowerRow.Cells.Add(c7);
            lowerRow.Cells.Add(c8);
            lowerRow.Cells.Add(c9);
            lowerRow.Cells.Add(c10);
            lowerRow.Cells.Add(c11);
            lowerRow.Cells.Add(c12);

            gv.Controls[0].Controls.RemoveAt(1);
            gv.Controls[0].Controls.AddAt(1, lowerRow);

            #endregion

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row3 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row4 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row5 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Branch:";
            cell_1.BackColor = Color.Yellow;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = CompanyRepository.BranchName(branch);
            cell_11.ColumnSpan = 12;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "File Handler:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = "All";
            cell_22.ColumnSpan = 12;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "Nature:";
            cell_3.BackColor = Color.Yellow;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = BusinessNatureRepository.NatureName(natid);
            cell_33.ColumnSpan = 12;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Request Date:";
            cell_4.BackColor = Color.Yellow;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = "From:" + Convert.ToDateTime(dateFrom).ToString("dd MMM yyyy") + " To: " + Convert.ToDateTime(dateTo).ToString("dd MMM yyyy");
            cell_44.ColumnSpan = 12;
            row4.Cells.Add(cell_44);

            TableCell cell_5 = new TableCell();
            cell_5.Text = "Country:";
            cell_5.BackColor = Color.Yellow;
            row5.Cells.Add(cell_5);

            TableCell cell_55 = new TableCell();
            cell_55.Text = CountryRepository.CountryName(country);
            cell_55.ColumnSpan = 12;
            row5.Cells.Add(cell_55);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);
            gv.Controls[0].Controls.AddAt(4, row5);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MarketAnalysis.xls");

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

        #endregion

        #region ---  Confirm Calender ---

        //[ActionAuthorize]
        public ActionResult ConfirmCalenderIndex()
        {
            EmployeeRepository e = new EmployeeRepository();
            CompanyRepository c = new CompanyRepository();
            if (User.IsInRole("User"))
            {
                ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.AppLogin.ToLower() == User.Identity.Name.ToLower()), "Id", "EmpName");
                ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    int deptid = e.GetManagerDept(User.Identity.Name.ToUpper());
                    ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.Department.Id == deptid), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
                }
                else
                {
                    ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList(), "Id", "Name");
                }
            }


            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        public ActionResult ConfirmCalender(string dateFrom = null, string dateTo = null, int branch = 0, int empid = 0, int natid = 0)
        {
            //List<NBOModel> model = new List<NBOModel>();
            NboCalendarModel model = new NboCalendarModel();
            model = ConfirmCalanderData(dateFrom, dateTo, branch, empid, natid);
            return PartialView(model);
        }

        public NboCalendarModel ConfirmCalanderData(string dateFrom = null, string dateTo = null, int branch = 0, int empid = 0, int natid = 0)
        {
            NboCalendarModel model1 = new NboCalendarModel();
            NBORepository dal = new NBORepository();
            List<NBOModel> model = new List<NBOModel>();

            if (empid != 0 && natid == 0)
            {
                model = dal.GetBy(empid).ToList();
            }
            if (empid != 0 && natid != 0)
            {
                model = dal.GetBy(empid, natid).ToList();
            }
            if (empid == 0 && natid != 0)
            {
                if (User.IsInRole("Manager"))
                {
                    model = dal.GetByManager(User.Identity.Name.ToUpper(), natid).ToList();
                }
                else
                {
                    if (branch == 0)
                    {
                        model = dal.GetByNature(natid).ToList();
                    }
                    else
                    {
                        model = dal.GetByNature(natid).Where(x => x.Branch.Id == branch).ToList();
                    }

                }

            }
            if (empid == 0 && natid == 0)
            {
                if (User.IsInRole("Manager"))
                {
                    model = dal.GetByManager(User.Identity.Name.ToUpper()).ToList();
                }
                else
                {
                    model = dal.GetAll().ToList();
                    model = dal.GetAllByFilter(model, null, branch, empid, natid).ToList();
                }
            }
            //CONFIRMED, CONTRACTED,OPERATED,CLOSED
            model = model.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).ToList();
            model = model.Where(x => x.CheckinDate >= Convert.ToDateTime(dateFrom) && x.CheckinDate <= Convert.ToDateTime(dateTo)).ToList();

            model1.CurrentNBO = model;
            model1.Incoming = NboCalendarRepository.GetReceivable();

            return model1;
        }

        public ActionResult ExportConfirm(string dateFrom = null, string dateTo = null, int branch = 0, int empid = 0, int natid = 0)
        {
            //List<NBOModel> model = new List<NBOModel>();
            NboCalendarModel model = new NboCalendarModel();
            model = ConfirmCalanderData(dateFrom, dateTo, branch, empid, natid);

            #region -- Filtering data to fill table --

            var query = from n in model.CurrentNBO
                        join b in model.Incoming on n.Id equals b.nboid into mb
                        from incoming in mb.DefaultIfEmpty()
                        select new
                        {
                            EventMonth = n.EventMonth,
                            branch = n.Branch.Name,
                            ClientName = n.ClientName.Name,
                            Group = n.ContactName,
                            PaxNo = n.PaxNo,
                            Nature = n.Nature.Name,
                            FileHandler = n.FileHandler.EmpName,
                            Fileno = n.FileNumber,
                            CheckinDate = n.CheckinDate,
                            CheckOutDate = n.CheckOutDate,
                            ContractValue = n.ContractValue,
                            ContractCost = n.ContractCost,
                            Margin = n.Margin,
                            MarginP = n.MarginP,
                            Received = incoming == null ? 0 : incoming.Received,
                            Balance = incoming == null ? n.ContractValue : incoming.Balance,
                            Remarks = n.Remarks
                        };
            var data = from m in query.OrderBy(x => x.CheckinDate)
                       group m by m.EventMonth into g
                       select g;

            #endregion

            string[] month = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("Branch", typeof(string)));
            dt.Columns.Add(new DataColumn("Agent Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Group Name", typeof(string)));
            dt.Columns.Add(new DataColumn("PaxNo", typeof(string)));
            dt.Columns.Add(new DataColumn("Nature", typeof(string)));

            dt.Columns.Add(new DataColumn("FileHandler", typeof(string)));
            dt.Columns.Add(new DataColumn("FileNo", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckIn", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckOut", typeof(string)));

            dt.Columns.Add(new DataColumn("CV", typeof(string)));
            dt.Columns.Add(new DataColumn("TC", typeof(string)));
            dt.Columns.Add(new DataColumn("M", typeof(string)));
            dt.Columns.Add(new DataColumn("MP%", typeof(string)));

            dt.Columns.Add(new DataColumn("Received", typeof(string)));
            dt.Columns.Add(new DataColumn("Balance", typeof(string)));

            dt.Columns.Add(new DataColumn("Week1", typeof(string)));
            dt.Columns.Add(new DataColumn("Week2", typeof(string)));
            dt.Columns.Add(new DataColumn("Week3", typeof(string)));
            dt.Columns.Add(new DataColumn("Week4", typeof(string)));
            dt.Columns.Add(new DataColumn("Week5", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));


            #endregion

            #region -- Table internal Body --

            foreach (var item in data)
            {
                DataRow monthrow = dt.NewRow();
                monthrow["Branch"] = month[Convert.ToInt32(item.Key.Substring(0, 2))] + "-" + item.Key.Substring(4, 2);
                dt.Rows.Add(monthrow);

                foreach (var item1 in item.OrderBy(x => x.CheckinDate))
                {
                    DataRow dr = dt.NewRow();
                    dr["Branch"] = item1.branch;
                    dr["Agent Name"] = item1.ClientName;
                    dr["Group Name"] = item1.Group;
                    dr["PaxNo"] = item1.PaxNo;
                    dr["Nature"] = item1.Nature;

                    dr["FileHandler"] = item1.FileHandler;
                    dr["FileNo"] = item1.Fileno;
                    dr["CheckIn"] = Convert.ToDateTime(item1.CheckinDate).ToString("dd MMM yyyy");
                    dr["CheckOut"] = Convert.ToDateTime(item1.CheckOutDate).ToString("dd MMM yyyy");

                    dr["CV"] = Convert.ToDouble(item1.ContractValue).ToString("##,##0");
                    dr["TC"] = Convert.ToDouble(item1.ContractCost).ToString("##,##0");
                    dr["M"] = Convert.ToDouble(item1.Margin).ToString("##,##0");
                    dr["MP%"] = Convert.ToDouble(item1.MarginP).ToString("#0.00");

                    dr["Received"] = Convert.ToDouble(item1.Received).ToString("##,##0");
                    dr["Balance"] = Convert.ToDouble(item1.Balance).ToString("##,##0");

                    switch (MyExtension.GetWeekNumberOfMonth(Convert.ToDateTime(item1.CheckinDate)))
                    {
                        case 1:
                            {
                                dr["Week1"] = item1.PaxNo;
                                dr["Week2"] = "";
                                dr["Week3"] = "";
                                dr["Week4"] = "";
                                dr["Week5"] = "";
                            }
                            break;
                        case 2:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = item1.PaxNo;
                                dr["Week3"] = "";
                                dr["Week4"] = "";
                                dr["Week5"] = "";
                            }
                            break;
                        case 3:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = "";
                                dr["Week3"] = item1.PaxNo;
                                dr["Week4"] = "";
                                dr["Week5"] = "";
                            }
                            break;
                        case 4:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = "";
                                dr["Week3"] = "";
                                dr["Week4"] = item1.PaxNo;
                                dr["Week5"] = "";
                            }
                            break;
                        case 5:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = "";
                                dr["Week3"] = "";
                                dr["Week4"] = "";
                                dr["Week5"] = item1.PaxNo;
                            }
                            break;
                        default:
                            break;


                    };
                    dr["Remarks"] = item1.Remarks;
                    dt.Rows.Add(dr);
                }
                DataRow head2 = dt.NewRow();
                head2["Branch"] = "Total";
                head2["PaxNo"] = item.Sum(x => x.PaxNo);
                head2["CV"] = Convert.ToDouble(item.Sum(x => x.ContractValue)).ToString("##,##0");
                head2["TC"] = Convert.ToDouble(item.Sum(x => x.ContractCost)).ToString("##,##0");
                head2["M"] = Convert.ToDouble(item.Sum(x => x.Margin)).ToString("##,##0");
                head2["MP%"] = Convert.ToDouble(((item.Sum(x => x.Margin) / item.Sum(x => x.ContractValue)) * 100)).ToString("#0.00");
                head2["Received"] = Convert.ToDouble(item.Sum(x => x.Received)).ToString("##,##0");
                head2["Balance"] = Convert.ToDouble(item.Sum(x => x.Balance)).ToString("##,##0");
                dt.Rows.Add(head2);
            }
            DataRow head3 = dt.NewRow();
            head3["Branch"] = "Grand Total";
            head3["PaxNo"] = data.Sum(x => x.Sum(y => y.PaxNo));
            head3["CV"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.ContractValue))).ToString("##,##0");
            head3["TC"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.ContractCost))).ToString("##,##0");
            head3["M"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.Margin))).ToString("##,##0");
            head3["MP%"] = Convert.ToDouble(((data.Sum(x => x.Sum(y => y.Margin)) / data.Sum(x => x.Sum(y => y.ContractValue))) * 100)).ToString("#0.00");
            head3["Received"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.Received))).ToString("##,##0");
            head3["Balance"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.Balance))).ToString("##,##0");
            dt.Rows.Add(head3);

            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            gv.HeaderRow.BackColor = ColorTranslator.FromHtml("#2b1971");
            gv.HeaderRow.ForeColor = Color.White;
            gv.HeaderRow.Font.Bold = true;

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row3 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row4 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Branch:";
            cell_1.BackColor = Color.Yellow;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = CompanyRepository.BranchName(branch);
            cell_11.ColumnSpan = 20;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "File Handler:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = EmployeeRepository.EmployeeName(empid);
            cell_22.ColumnSpan = 20;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "Nature:";
            cell_3.BackColor = Color.Yellow;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = BusinessNatureRepository.NatureName(natid);
            cell_33.ColumnSpan = 20;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Event Date:";
            cell_4.BackColor = Color.Yellow;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = "From:" + Convert.ToDateTime(dateFrom).ToString("dd MMM yyyy") + " To: " + Convert.ToDateTime(dateTo).ToString("dd MMM yyyy");
            cell_44.ColumnSpan = 20;
            row4.Cells.Add(cell_44);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ConfirmCalendar.xls");

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

        #endregion

        #region ---  Active Calender ---

        //[ActionAuthorize]
        public ActionResult ActiveCalenderIndex()
        {
            ViewBag.Emp = EmpList();
            EmployeeRepository e = new EmployeeRepository();
            CompanyRepository c = new CompanyRepository();
            if (User.IsInRole("User"))
            {
                ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.AppLogin.ToLower() == User.Identity.Name.ToLower()), "Id", "EmpName");
                ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    int deptId = e.GetManagerDept(User.Identity.Name.ToUpper());
                    ViewBag.Emp = new SelectList(e.GetAll().Where(x => x.Department.Id == deptId), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList().Where(x => x.Id == e.GetBranchId(User.Identity.Name)), "Id", "Name");
                }
                else
                {
                    ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");
                    ViewBag.Branch = new SelectList(c.BranchList(), "Id", "Name");
                }

            }

            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        public ActionResult ActiveCalender(string dateFrom = null, string dateTo = null, int branch = 0, int empid = 0, int natid = 0)
        {
            //List<NBOModel> model = new List<NBOModel>();
            NboCalendarModel model = new NboCalendarModel();
            model = ActiveCalenderData(dateFrom, dateTo, branch, empid, natid);
            return PartialView(model);
        }

        public NboCalendarModel ActiveCalenderData(string dateFrom = null, string dateTo = null, int branch = 0, int empid = 0, int natid = 0)
        {
            NboCalendarModel model1 = new NboCalendarModel();
            NBORepository dal = new NBORepository();
            List<NBOModel> model = new List<NBOModel>();

            if (empid != 0 && natid == 0)
            {
                model = dal.GetBy(empid).ToList();
            }
            if (empid != 0 && natid != 0)
            {
                model = dal.GetBy(empid, natid).ToList();
            }
            if (empid == 0 && natid != 0)
            {
                if (User.IsInRole("Manager"))
                {
                    model = dal.GetByManager(User.Identity.Name.ToUpper(), natid).ToList();
                }
                else
                {
                    if (branch == 0)
                    {
                        model = dal.GetByNature(natid).ToList();
                    }
                    else
                    {
                        model = dal.GetByNature(natid).Where(x => x.Branch.Id == branch).ToList();
                    }
                }
            }
            if (empid == 0 && natid == 0)
            {
                if (User.IsInRole("Manager"))
                {
                    model = dal.GetByManager(User.Identity.Name.ToUpper()).ToList();
                }
                else
                {
                    model = dal.GetAll().ToList();
                    model = dal.GetAllByFilter(model, null, branch, empid, natid).ToList();
                }
            }
            //PROPOSAL STAGE, ACTIVE, POTENTIAL
            model = model.Where(x => x.Status.Id == 1 || x.Status.Id == 2 || x.Status.Id == 3).ToList();
            model = model.Where(x => x.CheckinDate >= Convert.ToDateTime(dateFrom) && x.CheckinDate <= Convert.ToDateTime(dateTo)).ToList();

            model1.CurrentNBO = model;
            model1.Incoming = NboCalendarRepository.GetReceivable();

            return model1;
        }

        public ActionResult ExportActive(string dateFrom = null, string dateTo = null, int branch = 0, int empid = 0, int natid = 0)
        {
            //List<NBOModel> model = new List<NBOModel>();
            NboCalendarModel model = new NboCalendarModel();
            model = ActiveCalenderData(dateFrom, dateTo, branch, empid, natid);

            #region -- Filtering data to fill table --

            var query = from n in model.CurrentNBO
                        join b in model.Incoming on n.Id equals b.nboid into mb
                        from incoming in mb.DefaultIfEmpty()
                        select new
                        {
                            EventMonth = n.EventMonth,
                            branch = n.Branch.Name,
                            ClientName = n.ClientName.Name,
                            Group = n.ContactName,
                            PaxNo = n.PaxNo,
                            Nature = n.Nature.Name,
                            FileHandler = n.FileHandler.EmpName,
                            Fileno = n.FileNumber,
                            CheckinDate = n.CheckinDate,
                            CheckOutDate = n.CheckOutDate,
                            ContractValue = n.ContractValue,
                            ContractCost = n.ContractCost,
                            Margin = n.Margin,
                            MarginP = n.MarginP,
                            Received = incoming == null ? 0 : incoming.Received,
                            Balance = incoming == null ? n.ContractValue : incoming.Balance,
                            Remarks = n.Remarks
                        };
            var data = from m in query.OrderBy(x => x.CheckinDate)
                       group m by m.EventMonth into g
                       select g;

            #endregion

            string[] month = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("Branch", typeof(string)));
            dt.Columns.Add(new DataColumn("Agent Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Group Name", typeof(string)));
            dt.Columns.Add(new DataColumn("PaxNo", typeof(string)));
            dt.Columns.Add(new DataColumn("Nature", typeof(string)));

            dt.Columns.Add(new DataColumn("FileHandler", typeof(string)));
            dt.Columns.Add(new DataColumn("FileNo", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckIn", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckOut", typeof(string)));

            dt.Columns.Add(new DataColumn("CV", typeof(string)));
            dt.Columns.Add(new DataColumn("TC", typeof(string)));
            dt.Columns.Add(new DataColumn("M", typeof(string)));
            dt.Columns.Add(new DataColumn("MP%", typeof(string)));

            dt.Columns.Add(new DataColumn("Received", typeof(string)));
            dt.Columns.Add(new DataColumn("Balance", typeof(string)));

            dt.Columns.Add(new DataColumn("Week1", typeof(string)));
            dt.Columns.Add(new DataColumn("Week2", typeof(string)));
            dt.Columns.Add(new DataColumn("Week3", typeof(string)));
            dt.Columns.Add(new DataColumn("Week4", typeof(string)));
            dt.Columns.Add(new DataColumn("Week5", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));


            #endregion

            #region -- Table internal Body --

            foreach (var item in data)
            {
                DataRow monthrow = dt.NewRow();
                monthrow["Branch"] = month[Convert.ToInt32(item.Key.Substring(0, 2))] + "-" + item.Key.Substring(4, 2);
                dt.Rows.Add(monthrow);

                foreach (var item1 in item.OrderBy(x => x.CheckinDate))
                {
                    DataRow dr = dt.NewRow();
                    dr["Branch"] = item1.branch;
                    dr["Agent Name"] = item1.ClientName;
                    dr["Group Name"] = item1.Group;
                    dr["PaxNo"] = item1.PaxNo;
                    dr["Nature"] = item1.Nature;

                    dr["FileHandler"] = item1.FileHandler;
                    dr["FileNo"] = item1.Fileno;
                    dr["CheckIn"] = Convert.ToDateTime(item1.CheckinDate).ToString("dd MMM yyyy");
                    dr["CheckOut"] = Convert.ToDateTime(item1.CheckOutDate).ToString("dd MMM yyyy");

                    dr["CV"] = Convert.ToDouble(item1.ContractValue).ToString("##,##0");
                    dr["TC"] = Convert.ToDouble(item1.ContractCost).ToString("##,##0");
                    dr["M"] = Convert.ToDouble(item1.Margin).ToString("##,##0");
                    dr["MP%"] = Convert.ToDouble(item1.MarginP).ToString("#0.00");

                    dr["Received"] = Convert.ToDouble(item1.Received).ToString("##,##0");
                    dr["Balance"] = Convert.ToDouble(item1.Balance).ToString("##,##0");

                    switch (MyExtension.GetWeekNumberOfMonth(Convert.ToDateTime(item1.CheckinDate)))
                    {
                        case 1:
                            {
                                dr["Week1"] = item1.PaxNo;
                                dr["Week2"] = "";
                                dr["Week3"] = "";
                                dr["Week4"] = "";
                                dr["Week5"] = "";
                            }
                            break;
                        case 2:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = item1.PaxNo;
                                dr["Week3"] = "";
                                dr["Week4"] = "";
                                dr["Week5"] = "";
                            }
                            break;
                        case 3:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = "";
                                dr["Week3"] = item1.PaxNo;
                                dr["Week4"] = "";
                                dr["Week5"] = "";
                            }
                            break;
                        case 4:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = "";
                                dr["Week3"] = "";
                                dr["Week4"] = item1.PaxNo;
                                dr["Week5"] = "";
                            }
                            break;
                        case 5:
                            {
                                dr["Week1"] = "";
                                dr["Week2"] = "";
                                dr["Week3"] = "";
                                dr["Week4"] = "";
                                dr["Week5"] = item1.PaxNo;
                            }
                            break;
                        default:
                            break;

                    };

                    dr["Remarks"] = item1.Remarks;
                    dt.Rows.Add(dr);
                }
                DataRow head2 = dt.NewRow();
                head2["Branch"] = "Total";
                head2["PaxNo"] = item.Sum(x => x.PaxNo);
                head2["CV"] = Convert.ToDouble(item.Sum(x => x.ContractValue)).ToString("##,##0");
                head2["TC"] = Convert.ToDouble(item.Sum(x => x.ContractCost)).ToString("##,##0");
                head2["M"] = Convert.ToDouble(item.Sum(x => x.Margin)).ToString("##,##0");
                head2["MP%"] = Convert.ToDouble(((item.Sum(x => x.Margin) / item.Sum(x => x.ContractValue)) * 100)).ToString("#0.00");
                head2["Received"] = Convert.ToDouble(item.Sum(x => x.Received)).ToString("##,##0");
                head2["Balance"] = Convert.ToDouble(item.Sum(x => x.Balance)).ToString("##,##0");
                dt.Rows.Add(head2);
            }
            DataRow head3 = dt.NewRow();
            head3["Branch"] = "Grand Total";
            head3["PaxNo"] = data.Sum(x => x.Sum(y => y.PaxNo));
            head3["CV"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.ContractValue))).ToString("##,##0");
            head3["TC"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.ContractCost))).ToString("##,##0");
            head3["M"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.Margin))).ToString("##,##0");
            head3["MP%"] = Convert.ToDouble(((data.Sum(x => x.Sum(y => y.Margin)) / data.Sum(x => x.Sum(y => y.ContractValue))) * 100)).ToString("#0.00");
            head3["Received"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.Received))).ToString("##,##0");
            head3["Balance"] = Convert.ToDouble(data.Sum(x => x.Sum(y => y.Balance))).ToString("##,##0");
            dt.Rows.Add(head3);

            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            gv.HeaderRow.BackColor = ColorTranslator.FromHtml("#2b1971");
            gv.HeaderRow.ForeColor = Color.White;
            gv.HeaderRow.Font.Bold = true;

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row3 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row4 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Branch:";
            cell_1.BackColor = Color.Yellow;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = CompanyRepository.BranchName(branch);
            cell_11.ColumnSpan = 20;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "File Handler:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = EmployeeRepository.EmployeeName(empid);
            cell_22.ColumnSpan = 20;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "Nature:";
            cell_3.BackColor = Color.Yellow;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = BusinessNatureRepository.NatureName(natid);
            cell_33.ColumnSpan = 20;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Event Date:";
            cell_4.BackColor = Color.Yellow;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = "From:" + Convert.ToDateTime(dateFrom).ToString("dd MMM yyyy") + " To: " + Convert.ToDateTime(dateTo).ToString("dd MMM yyyy");
            cell_44.ColumnSpan = 20;
            row4.Cells.Add(cell_44);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ActiveCalendar.xls");

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

        #endregion

        #region -- NBO Hotels ---

        [HttpPost]
        public JsonResult AddHotel(HotelDetailsModel model, int nboid)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                HotelDetailsRepository dal = new HotelDetailsRepository();
                dal.AddHotel(model, nboid);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditHotel(HotelDetailsModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                HotelDetailsRepository dal = new HotelDetailsRepository();
                dal.Edit(model);
                HotelDetailsModel data = dal.GetById(model.Id);
                return Json(new { Result = "OK", Record = data });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListHotel(int nboid)
        {
            try
            {
                HotelDetailsRepository dal = new HotelDetailsRepository();
                var model = dal.GetNBOHotel(nboid);
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteHotel(HotelDetailsModel model)
        {
            try
            {
                HotelDetailsRepository dal = new HotelDetailsRepository();
                dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region -- Representation --

        public ActionResult RepresentationAnalysisIndex()
        {
            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().OrderBy(x => x.Name), "Id", "Name");

            CountryRepository c = new CountryRepository();
            ViewBag.Market = new SelectList(c.GetAll().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        public AnayalisModel RepresentationData(string enquiry = null, int country = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            List<NBOModel> model = new List<NBOModel>();
            NBORepository dal = new NBORepository();

            AnayalisModel anayalis = new AnayalisModel();
            AnaylysisRepository rep = new AnaylysisRepository();

            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                model = dal.GetAll().ToList();

                if (country != 0 && enquiry == "")
                {
                    model = model.Where(x => x.ClientCountry.Id == country).ToList();
                }
                if (country == 0 && enquiry != "")
                {
                    string[] list = enquiry.Split(',');
                    model = model.Where(x => list.Contains(x.EnquirySource.Id.ToString())).ToList();
                }
                if (natid != 0 && enquiry == "")
                {
                    if (natid == 6)
                    {
                        model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                    }
                    else
                    {
                        model = model.Where(x => x.Nature.Id == natid).ToList();
                    }
                }
                if (natid != 0 && enquiry != "")
                {
                    if (natid == 6)
                    {
                        model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                    }
                    else
                    {
                        model = model.Where(x => x.Nature.Id == natid).ToList();
                    }
                    string[] list = enquiry.Split(',');
                    model = model.Where(x => list.Contains(x.EnquirySource.Id.ToString())).ToList();
                }
                if (country != 0 && enquiry != "")
                {
                    model = model.Where(x => x.ClientCountry.Id == country).ToList();
                    string[] list = enquiry.Split(',');
                    model = model.Where(x => list.Contains(x.EnquirySource.Id.ToString())).ToList();
                }
                if (country != 0 && enquiry == "")
                {
                    model = model.Where(x => x.ClientCountry.Id == country).ToList();
                }
            }
            else
            {
                model = dal.GetAll().Take(0).ToList();
            }

            anayalis.CurrentNBO = rep.GetCurrentNBO(model, dateFrom, dateTo);
            anayalis.PriviousNBO = rep.GetPriviousNBO(model, dateFrom, dateTo);

            return anayalis;

        }

        public ActionResult RepresentationAnalysis(string enquiry = null, int country = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            AnayalisModel anayalis = new AnayalisModel();
            anayalis = RepresentationData(enquiry, country, dateFrom, dateTo, natid);
            return PartialView(anayalis);
            //return PartialView(model);
        }

        public ActionResult ExportRepresentation(string enquiry = null, int country = 0, string dateFrom = null, string dateTo = null, int natid = 0)
        {
            AnayalisModel model = new AnayalisModel();
            model = RepresentationData(enquiry, country, dateFrom, dateTo, natid);

            #region -- Filtering data to fill table --

            var currentC = from c in model.CurrentNBO
                           select new { Client = c.ClientName.Name, Country = c.ClientCountry.Name, Enquiry = c.EnquirySource.Name };
            var priviousC = from c in model.PriviousNBO
                            select new { Client = c.ClientName.Name, Country = c.ClientCountry.Name, Enquiry = c.EnquirySource.Name };

            var cont = currentC.Concat(priviousC).Distinct();

            var details = from n in model.CurrentNBO
                          group n by new { Clinet = n.ClientName.Name, Country = n.ClientCountry.Name, Enquiry = n.EnquirySource.Name } into s
                          select new
                          {
                              Client = s.Key.Clinet,
                              Country = s.Key.Country,
                              Enquiry = s.Key.Enquiry,
                              //Active/Potantial
                              ActiveValue = s.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Sum(x => x.ContractValue),
                              ActiveFiles = s.Where(x => x.Status.Id == 2 || x.Status.Id == 3).Count(),
                              //Cancelled /Inactive
                              CancelledValue = s.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Sum(x => x.ContractValue),
                              CancelledFiles = s.Where(x => x.Status.Id == 8 || x.Status.Id == 9).Count(),
                              //Confirmed/Contracted/Operated/Closed
                              ConfirmendValue = s.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Sum(x => x.ContractValue),
                              ConfirmendFiles = s.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).Count(),
                              //ProposalStage
                              ProposalStage = s.Where(x => x.Status.Id == 1).Sum(x => x.ContractValue),
                              ProposalFiles = s.Where(x => x.Status.Id == 1).Count(),
                              TotalFiles = s.Count(),
                              TotalValue = s.Sum(x => x.ContractValue),
                          };

            var privious = from n in model.PriviousNBO
                           group n by new { Client = n.ClientName.Name, Country = n.ClientCountry.Name, Enquiry = n.EnquirySource.Name } into s
                           select new
                           {
                               Client = s.Key.Client,
                               Country = s.Key.Country,
                               Enquiry = s.Key.Enquiry,
                               TotalFiles = s.Count(),
                               TotalValue = s.Sum(x => x.ContractValue),
                           };

            var query = from c in cont
                        join m in details on new { c.Client, c.Country, c.Enquiry } equals new { m.Client, m.Country, m.Enquiry } into mb
                        join b in privious on new { c.Client, c.Country, c.Enquiry } equals new { b.Client, b.Country, b.Enquiry } into pv
                        from current in mb.DefaultIfEmpty()
                        from pri in pv.DefaultIfEmpty()
                        select new
                        {
                            Client = c.Client,
                            Country = c.Country,
                            Enquiry = c.Enquiry,
                            ActiveValue = current == null ? 0 : current.ActiveValue,
                            ActiveFiles = current == null ? 0 : current.ActiveFiles,
                            CancelledValue = current == null ? 0 : current.CancelledValue,
                            CancelledFiles = current == null ? 0 : current.CancelledFiles,
                            ConfirmendValue = current == null ? 0 : current.ConfirmendValue,
                            ConfirmendFiles = current == null ? 0 : current.ConfirmendFiles,
                            ProposalStage = current == null ? 0 : current.ProposalStage,
                            ProposalFiles = current == null ? 0 : current.ProposalFiles,
                            TotalFiles = current == null ? 0 : current.TotalFiles,
                            TotalValue = current == null ? 0 : current.TotalValue,
                            LastFiles = (pri == null ? 0 : pri.TotalFiles),
                            LastValues = (pri == null ? 0 : pri.TotalValue)
                        };

            #endregion

            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("Client", typeof(string)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));
            dt.Columns.Add(new DataColumn("Representation", typeof(string)));

            dt.Columns.Add(new DataColumn("Pro-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Pro-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Act-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Act-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Con-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Con-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Can-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Can-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Total-Files", typeof(string)));
            dt.Columns.Add(new DataColumn("Total-Values", typeof(string)));

            dt.Columns.Add(new DataColumn("Last-TotalFiles", typeof(string)));
            dt.Columns.Add(new DataColumn("Last-TotalValues", typeof(string)));

            #endregion

            #region -- Table internal Body --

            foreach (var item in query.OrderBy(x => x.Country))
            {
                DataRow dr = dt.NewRow();
                dr["Client"] = item.Client;
                dr["Country"] = item.Country;
                dr["Representation"] = item.Enquiry;

                dr["Pro-Files"] = item.ProposalFiles;
                dr["Pro-Values"] = item.ProposalStage;

                dr["Act-Files"] = item.ActiveFiles;
                dr["Act-Values"] = item.ActiveValue;

                dr["Con-Files"] = item.ConfirmendFiles;
                dr["Con-Values"] = item.ConfirmendValue;

                dr["Can-Files"] = item.CancelledFiles;
                dr["Can-Values"] = item.CancelledValue;

                dr["Total-Files"] = item.TotalFiles;
                dr["Total-Values"] = item.TotalValue;

                dr["Last-TotalFiles"] = item.LastFiles;
                dr["Last-TotalValues"] = item.LastValues;

                dt.Rows.Add(dr);
            }

            #endregion

            #region --- BPR table footer --

            DataRow f = dt.NewRow();
            f["Country"] = "Total";

            f["Pro-Files"] = query.Sum(x => x.ProposalFiles);
            f["Pro-Values"] = query.Sum(x => x.ProposalStage);

            f["Act-Files"] = query.Sum(x => x.ActiveFiles);
            f["Act-Values"] = query.Sum(x => x.ActiveValue);

            f["Con-Files"] = query.Sum(x => x.ConfirmendFiles);
            f["Con-Values"] = query.Sum(x => x.ConfirmendValue);

            f["Can-Files"] = query.Sum(x => x.CancelledFiles);
            f["Can-Values"] = query.Sum(x => x.CancelledValue);

            f["Total-Files"] = query.Sum(x => x.TotalFiles);
            f["Total-Values"] = query.Sum(x => x.TotalValue);

            f["Last-TotalFiles"] = query.Sum(x => x.LastFiles);
            f["Last-TotalValues"] = query.Sum(x => x.LastValues);

            dt.Rows.Add(f);

            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            #region -- Table Top Header ---

            GridViewRow headerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell = new TableCell();
            cell.Text = "Client";
            cell.RowSpan = 2;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.BackColor = Color.Silver;
            headerRow.Cells.Add(cell);

            TableCell cell55 = new TableCell();
            cell55.Text = "Country";
            cell55.RowSpan = 2;
            cell55.HorizontalAlign = HorizontalAlign.Center;
            cell55.BackColor = Color.Silver;
            headerRow.Cells.Add(cell55);

            TableCell cell66 = new TableCell();
            cell66.Text = "Representation";
            cell66.RowSpan = 2;
            cell66.HorizontalAlign = HorizontalAlign.Center;
            cell66.BackColor = Color.Silver;
            headerRow.Cells.Add(cell66);


            TableCell cell2 = new TableCell();
            cell2.Text = "Proposal Stage";
            cell2.ColumnSpan = 2;
            cell2.HorizontalAlign = HorizontalAlign.Center;
            cell2.BackColor = Color.Yellow;
            headerRow.Cells.Add(cell2);

            TableCell cell3 = new TableCell();
            cell3.Text = "Active/Potantial";
            cell3.ColumnSpan = 2;
            cell3.HorizontalAlign = HorizontalAlign.Center;
            cell3.BackColor = Color.LightGreen;
            headerRow.Cells.Add(cell3);

            TableCell cell4 = new TableCell();
            cell4.Text = "Confirmed / Contracted / Operated / Closed";
            cell4.ColumnSpan = 2;
            cell4.Wrap = true;
            cell4.HorizontalAlign = HorizontalAlign.Center;
            cell4.BackColor = Color.LightBlue;
            headerRow.Cells.Add(cell4);

            TableCell cell5 = new TableCell();
            cell5.Text = "Cancelled /InActive";
            cell5.ColumnSpan = 2;
            cell5.HorizontalAlign = HorizontalAlign.Center;
            cell5.BackColor = Color.Red;
            headerRow.Cells.Add(cell5);

            TableCell cell6 = new TableCell();
            cell6.Text = "Total";
            cell6.ColumnSpan = 2;
            cell6.HorizontalAlign = HorizontalAlign.Center;
            cell6.BackColor = Color.Violet;
            headerRow.Cells.Add(cell6);

            TableCell cell7 = new TableCell();
            cell7.Text = "Last Year Total";
            cell7.ColumnSpan = 2;
            cell7.HorizontalAlign = HorizontalAlign.Center;
            cell7.BackColor = Color.Blue;
            headerRow.Cells.Add(cell7);

            gv.Controls[0].Controls.AddAt(0, headerRow);

            GridViewRow lowerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell c1 = new TableCell();
            TableCell c2 = new TableCell();
            TableCell c3 = new TableCell();
            TableCell c4 = new TableCell();
            TableCell c5 = new TableCell();
            TableCell c6 = new TableCell();
            TableCell c7 = new TableCell();
            TableCell c8 = new TableCell();
            TableCell c9 = new TableCell();
            TableCell c10 = new TableCell();
            TableCell c11 = new TableCell();
            TableCell c12 = new TableCell();

            c1.BackColor = Color.Silver;
            c2.BackColor = Color.Silver;
            c3.BackColor = Color.Silver;
            c4.BackColor = Color.Silver;
            c5.BackColor = Color.Silver;
            c6.BackColor = Color.Silver;
            c7.BackColor = Color.Silver;
            c8.BackColor = Color.Silver;
            c9.BackColor = Color.Silver;
            c10.BackColor = Color.Silver;
            c11.BackColor = Color.Silver;
            c12.BackColor = Color.Silver;

            c1.Text = "No";
            c1.Width = new Unit(50);
            c2.Text = "Value";
            c2.Width = new Unit(80);
            c3.Text = "No";
            c3.Width = new Unit(50);
            c4.Text = "Value";
            c4.Width = new Unit(80);
            c5.Text = "No";
            c5.Width = new Unit(50);
            c6.Text = "Value";
            c6.Width = new Unit(80);
            c7.Text = "No";
            c7.Width = new Unit(50);
            c8.Text = "Value";
            c8.Width = new Unit(80);
            c9.Text = "No";
            c9.Width = new Unit(50);
            c10.Text = "Value";
            c10.Width = new Unit(80);
            c11.Text = "No";
            c11.Width = new Unit(50);
            c12.Text = "Value";
            c12.Width = new Unit(80);

            lowerRow.Cells.Add(c1);
            lowerRow.Cells.Add(c2);
            lowerRow.Cells.Add(c3);
            lowerRow.Cells.Add(c4);
            lowerRow.Cells.Add(c5);
            lowerRow.Cells.Add(c6);
            lowerRow.Cells.Add(c7);
            lowerRow.Cells.Add(c8);
            lowerRow.Cells.Add(c9);
            lowerRow.Cells.Add(c10);
            lowerRow.Cells.Add(c11);
            lowerRow.Cells.Add(c12);

            gv.Controls[0].Controls.RemoveAt(1);
            gv.Controls[0].Controls.AddAt(1, lowerRow);

            #endregion

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row3 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row4 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row5 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Branch:";
            cell_1.BackColor = Color.Yellow;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = CompanyRepository.BranchName(0);
            cell_11.ColumnSpan = 14;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "Market:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = CountryRepository.Country(country);
            cell_22.ColumnSpan = 14;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "Nature:";
            cell_3.BackColor = Color.Yellow;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = BusinessNatureRepository.NatureName(natid);
            cell_33.ColumnSpan = 14;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Request Date:";
            cell_4.BackColor = Color.Yellow;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = "From:" + Convert.ToDateTime(dateFrom).ToString("dd MMM yyyy") + " To: " + Convert.ToDateTime(dateTo).ToString("dd MMM yyyy");
            cell_44.ColumnSpan = 14;
            row4.Cells.Add(cell_44);

            TableCell cell_5 = new TableCell();
            cell_5.Text = "Country:";
            cell_5.BackColor = Color.Yellow;
            row5.Cells.Add(cell_5);

            TableCell cell_55 = new TableCell();
            cell_55.Text = EnquirySourceRepository.Enquiry(enquiry);
            cell_55.ColumnSpan = 14;
            row5.Cells.Add(cell_55);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);
            gv.Controls[0].Controls.AddAt(4, row5);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=RepresentatonAnalysis.xls");

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

        #endregion

        private SelectList EmpList()
        {
            EmployeeRepository e = new EmployeeRepository();
            SelectList list;
            if (User.IsInRole("User"))
            {
                list = new SelectList(e.GetAll().Where(x => x.AppLogin.ToLower() == User.Identity.Name.ToLower()), "Id", "EmpName");
            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    int groupId = e.GetManagerDept(User.Identity.Name.ToUpper());
                    list = new SelectList(e.GetAll().Where(x => x.GroupId == groupId), "Id", "EmpName");
                }
                else
                {
                    list = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");
                }
            }
            return list;
        }

        public ActionResult ExportData()
        {
            string[] month = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Branch", typeof(string)));
            dt.Columns.Add(new DataColumn("FileHandler", typeof(string)));
            dt.Columns.Add(new DataColumn("Nature", typeof(string)));
            dt.Columns.Add(new DataColumn("RequestDate", typeof(string)));
            dt.Columns.Add(new DataColumn("FileNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("Client", typeof(string)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));
            dt.Columns.Add(new DataColumn("EventName", typeof(string)));
            dt.Columns.Add(new DataColumn("Pax", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckIn", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckOut", typeof(string)));
            dt.Columns.Add(new DataColumn("Representation", typeof(string)));
            dt.Columns.Add(new DataColumn("ContractValue", typeof(double)));
            dt.Columns.Add(new DataColumn("ContractCost", typeof(double)));
            dt.Columns.Add(new DataColumn("Margin", typeof(double)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));

            GridView gv = new GridView();
            NBORepository dal = new NBORepository();
            var data = exportList.OrderBy(x => x.EventMonth).ToList();

            foreach (var item1 in data.GroupBy(x => x.EventMonth).ToList())
            {
                DataRow drmonth = dt.NewRow();
                drmonth["Branch"] = month[Convert.ToInt32(item1.Key.Substring(0, 2))] + "-" + item1.Key.Substring(4, 2);
                dt.Rows.Add(drmonth);

                foreach (var item in item1)
                {
                    DataRow dr = dt.NewRow();
                    dr["Branch"] = item.Branch.Name;
                    dr["FileHandler"] = item.FileHandler.EmpName;
                    dr["Nature"] = item.Nature.Name;
                    dr["RequestDate"] = item.RequestDate.ToString("dd-MM-yyyy");
                    dr["FileNumber"] = item.FileNumber;
                    dr["Client"] = item.ClientName.Name;
                    dr["Country"] = item.ClientCountry.Name;
                    dr["EventName"] = item.EventName;
                    dr["Pax"] = item.PaxNo;
                    dr["CheckIn"] = Convert.ToDateTime(item.CheckinDate).ToString("dd-MM-yyyy");
                    dr["CheckOut"] = Convert.ToDateTime(item.CheckOutDate).ToString("dd-MM-yyyy");
                    dr["Representation"] = item.EnquirySource.Name;
                    dr["ContractValue"] = item.ContractValue;
                    dr["ContractCost"] = item.ContractCost;
                    dr["Margin"] = Convert.ToDouble(item.ContractValue) - Convert.ToDouble(item.ContractCost);
                    dr["Status"] = item.Status.Name;
                    dr["Remarks"] = item.Remarks;
                    dt.Rows.Add(dr);
                }
                DataRow MonthTotal = dt.NewRow();
                MonthTotal["Branch"] = "Month Total";
                MonthTotal["FileNumber"] = item1.Count();
                MonthTotal["Pax"] = item1.Sum(x => x.PaxNo);
                MonthTotal["ContractValue"] = Convert.ToDouble(item1.Sum(x => x.ContractValue)).ToString("##,###.00");
                MonthTotal["ContractCost"] = Convert.ToDouble(item1.Sum(x => x.ContractCost)).ToString("##,###.00");
                MonthTotal["Margin"] = Convert.ToDouble(item1.Sum(x => x.Margin)).ToString("##,###.00");
                dt.Rows.Add(MonthTotal);
            }
            DataRow GrandTotal = dt.NewRow();
            GrandTotal["Branch"] = "Grand Total";
            GrandTotal["FileNumber"] = data.Count();
            GrandTotal["Pax"] = data.Sum(x => x.PaxNo);
            GrandTotal["ContractValue"] = Convert.ToDouble(data.Sum(x => x.ContractValue)).ToString("##,###.00");
            GrandTotal["ContractCost"] = Convert.ToDouble(data.Sum(x => x.ContractCost)).ToString("##,###.00");
            GrandTotal["Margin"] = Convert.ToDouble(data.Sum(x => x.Margin)).ToString("##,###.00");
            dt.Rows.Add(GrandTotal);

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = "NBOData.xls";
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

    }
}
