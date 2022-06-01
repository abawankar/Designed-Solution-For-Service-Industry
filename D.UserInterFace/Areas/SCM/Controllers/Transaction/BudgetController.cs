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
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Areas.SCM.Models.Report;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    [HandleError]
    
    public class BudgetController : Controller
    {
        //
        // GET: /Budget/

        //[ActionAuthorize]
        public ActionResult Index()
        {
            SelectList year = new SelectList(Years(), "Text", "Value");
            ViewBag.Year = year;

            CompanyRepository c = new CompanyRepository();
            ViewBag.branch = new SelectList(c.GetById(1).Branches.OrderBy(x => x.Name), "Id", "Name");

            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().Where(x => x.Id != 1 && x.Id != 2).OrderBy(x => x.Name), "Id", "Name");

            EmployeeRepository e = new EmployeeRepository();
            ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");

            return View();
        }

        #region ---- Budget -----

        [Authorize]
        [HttpPost]
        public JsonResult List(int branch = 0, string col = null, string year = null, int emp = 0, int nature = 0, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                BudgetRepository dal = new BudgetRepository();
                List<BudgetModel> model = new List<BudgetModel>();
                if (!String.IsNullOrEmpty(col))
                {
                    switch (col)
                    {
                        case "branch":
                            model = dal.GetAll().Where(x => x.Branch.Id == branch).ToList();
                            break;
                        case "year":
                            model = dal.GetAll().Where(x => x.Year == year).ToList();
                            break;
                        case "emp":
                            model = dal.GetAll().Where(x => x.Employee.Id == emp).ToList();
                            break;
                        case "nature":
                            model = dal.GetAll().Where(x => x.Nature.Id == nature).ToList();
                            break;
                        case "filter":
                            model = dal.GetAllByFilter(year, branch, emp, nature).ToList();
                            break;
                        default:
                            model = dal.GetAll().Take(200).ToList();
                            break;
                    }
                }
                else
                {
                    model = dal.GetAll().Take(200).ToList();
                }


                int count = model.Count;
                List<BudgetModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new Budget
       
        [HttpPost]
        public JsonResult Create(BudgetModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                BudgetRepository dal = new BudgetRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update Budget details
       
        [HttpPost]
        public JsonResult Update(BudgetModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                BudgetRepository dal = new BudgetRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ListBudgetTrn(int butdgetId)
        {
            try
            {
                BudgetRepository dal = new BudgetRepository();
                var model = dal.GetById(butdgetId).BudTrn.ToList();
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update NBO details
        [Authorize]
        [HttpPost]
        public JsonResult EditBudgetTrn(BudgetTrnModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                BudgetRepository dal = new BudgetRepository();
                dal.EditBudgetTrn(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetYearOptions()
        {
            try
            {
                string[] year = { "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020" };
                var list = year;
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private static IEnumerable<SelectListItem> Years()
        {
            List<SelectListItem> year = new List<SelectListItem>();
            for (int i = 2010; i < 2020; i++)
            {
                year.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            return year;
        }

        #endregion

        #region ---- Report ----

        //[ActionAuthorize]
        public ActionResult BPRReport(string year = null, int branch = 0, int empid = 0, int natid = 0)
        {
            BPRModel model = new BPRModel();
            model = BPRData(year, branch, empid, natid);
            return PartialView(model);
        }

        public BPRModel BPRData(string year = null, int branch = 0, int empid = 0, int natid = 0)
        {
            DateTime dt = DateTime.Now;
            ViewBag.Year = year;
            BPRModel model = new BPRModel();
            BPRRepository dal = new BPRRepository();
            EmployeeRepository e = new EmployeeRepository();
            if (User.IsInRole("User"))
            {
                if (string.IsNullOrEmpty(year))
                {
                    year = dt.Year.ToString();
                    ViewBag.Year = year;
                }

                empid = e.GetByName(User.Identity.Name.ToUpper());

            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    if (string.IsNullOrEmpty(year))
                    {
                        year = dt.Year.ToString();
                        ViewBag.Year = year;
                    }
                }
            }

            if (!string.IsNullOrEmpty(year))
            {
                if (natid != 0 && empid != 0)
                {
                    model = dal.GetBy(year, empid, natid);
                }
                if (natid != 0 && empid == 0)
                {
                    if (User.IsInRole("Manager"))
                    {
                        model = dal.GetBYManager(year, natid, User.Identity.Name.ToUpper());
                    }
                    else
                    {
                        model = dal.GetByNature(year, natid, branch);
                    }
                }
                if (empid != 0 && natid == 0)
                {
                    model = dal.GetByEmployee(year, empid);
                }
                if (empid == 0 && natid == 0)
                {
                    if (User.IsInRole("Manager"))
                    {
                        model = dal.GetBYManager(year, User.Identity.Name.ToUpper());
                    }
                    else
                    {
                        model = dal.GetBy(year, branch);
                    }
                }
            }
            else
            {
                year = dt.Year.ToString();
                ViewBag.Year = year;
                model = dal.GetBy(year, branch);
            }

            return model;
        }

        public ActionResult Export(string year = null, int branch = 0, int empid = 0, int natid = 0)
        {
            BPRModel model = new BPRModel();
            model = BPRData(year, branch, empid, natid);

            #region -- Filtering data to fill table --

            var data = from m in model.Budget
                       from n in m.BudTrn.ToList()
                       select new { bmonth = n.BudgetMonth, year = m.Year, Month = n.Month, s = n.ContractValue, c = n.ContractCost, m = n.Margin };
            var budget = from x in data.GroupBy(x => x.bmonth)
                         select new { month = x.Key, s = x.Sum(s => s.s), c = x.Sum(c => c.c), m = x.Sum(m => m.m) };

            //CONFIRMED,CONTRACTED,OPERATED,CLOSED
            var confirm = from m in model.NBO.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).GroupBy(y => y.EventMonth)
                          select new { month = m.Key, s = m.Sum(s => s.ContractValue), c = m.Sum(c => c.ContractCost), m = m.Sum(a => a.Margin) };

            //Active and Potential
            var active = from m in model.NBO.Where(x => x.Status.Id == 2 || x.Status.Id == 3).GroupBy(y => y.EventMonth)
                         select new { month = m.Key, s = m.Sum(s => s.ContractValue), c = m.Sum(c => c.ContractCost), m = m.Sum(a => a.Margin) };

            //Inacative and Cancelled       
            var cancelled = from m in model.NBO.Where(x => x.Status.Id == 8 || x.Status.Id == 9).GroupBy(y => y.EventMonth)
                            select new { month = m.Key, s = m.Sum(s => s.ContractValue), c = m.Sum(c => c.ContractCost), m = m.Sum(a => a.Margin) };

            var query = from m in model.month
                        join b in budget on m.Month equals b.month into mb
                        join c in confirm on m.Month equals c.month into cd
                        join l in cancelled on m.Month equals l.month into cl
                        join a in active on m.Month equals a.month into ac
                        from bbudget in mb.DefaultIfEmpty()
                        from sconfirm in cd.DefaultIfEmpty()
                        from scancelled in cl.DefaultIfEmpty()
                        from sactive in ac.DefaultIfEmpty()
                        select new
                        {
                            month = m.Month,
                            bs = (bbudget == null ? 0 : bbudget.s),
                            bc = (bbudget == null ? 0 : bbudget.c),
                            bm = (bbudget == null ? 0 : bbudget.m),
                            cs = (sconfirm == null ? 0 : sconfirm.s),
                            cc = (sconfirm == null ? 0 : sconfirm.c),
                            cm = (sconfirm == null ? 0 : sconfirm.m),
                            ls = (scancelled == null ? 0 : scancelled.s),
                            lc = (scancelled == null ? 0 : scancelled.c),
                            lm = (scancelled == null ? 0 : scancelled.m),
                            acs = (sactive == null ? 0 : sactive.s),
                            acc = (sactive == null ? 0 : sactive.c),
                            acm = (sactive == null ? 0 : sactive.m)
                        };

            #endregion

            string[] month = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("Month", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetValue", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetCost", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetMargin", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetMarginP", typeof(string)));

            dt.Columns.Add(new DataColumn("Value", typeof(string)));
            dt.Columns.Add(new DataColumn("Cost", typeof(string)));
            dt.Columns.Add(new DataColumn("Margin", typeof(string)));
            dt.Columns.Add(new DataColumn("MarginP", typeof(string)));

            dt.Columns.Add(new DataColumn("ActiveValue", typeof(string)));
            dt.Columns.Add(new DataColumn("ActiveCost", typeof(string)));
            dt.Columns.Add(new DataColumn("ActiveMargin", typeof(string)));
            dt.Columns.Add(new DataColumn("ActiveMarginP", typeof(string)));

            dt.Columns.Add(new DataColumn("CancelledValue", typeof(string)));
            dt.Columns.Add(new DataColumn("CancelledCost", typeof(string)));
            dt.Columns.Add(new DataColumn("CancelledMargin", typeof(string)));
            dt.Columns.Add(new DataColumn("CancelledMarginP", typeof(string)));

            #endregion

            #region -- Table internal Body --

            foreach (var item in query)
            {
                DataRow dr = dt.NewRow();
                dr["Month"] = month[Convert.ToInt32(item.month.Substring(0, 2)) - 1].ToString();
                dr["TargetValue"] = Convert.ToDouble(item.bs).ToString("##,##0");
                dr["TargetCost"] = Convert.ToDouble(item.bc).ToString("##,##0");
                dr["TargetMargin"] = Convert.ToDouble(item.bm).ToString("##,##0");
                dr["TargetMarginP"] = (item.bs == 0 || item.bs == null) ? "0" : Convert.ToDouble((item.bm / item.bs) * 100).ToString("#0.00");

                dr["Value"] = Convert.ToDouble(item.cs).ToString("##,##0");
                dr["Cost"] = Convert.ToDouble(item.cc).ToString("##,##0");
                dr["Margin"] = Convert.ToDouble(item.cm).ToString("##,##0");
                dr["MarginP"] = (item.cs == 0 || item.cs == null) ? "0" : Convert.ToDouble((item.cm / item.cs) * 100).ToString("#0.00");

                dr["ActiveValue"] = item.acs;
                dr["ActiveCost"] = item.acc;
                dr["ActiveMargin"] = item.acm;
                dr["ActiveMarginP"] = (item.acs == 0 || item.acs == null) ? 0 : (item.acm / item.acs) * 100;

                dr["CancelledValue"] = item.ls;
                dr["CancelledCost"] = item.lc;
                dr["CancelledMargin"] = item.lm;
                dr["CancelledMarginP"] = (item.ls == 0 || item.ls == null) ? 0 : (item.lm / item.ls) * 100;

                dt.Rows.Add(dr);
            }

            #endregion

            #region --- BPR table footer --

            DataRow f = dt.NewRow();
            f["Month"] = "Total";

            f["TargetValue"] = query.Sum(x => x.bs);
            f["TargetCost"] = query.Sum(x => x.bc);
            f["TargetMargin"] = query.Sum(x => x.bm);
            f["TargetMarginP"] = query.Sum(x => x.bs) == 0 ? 0 : (query.Sum(x => x.bm) / query.Sum(x => x.bs)) * 100;

            f["Value"] = query.Sum(x => x.cs);
            f["Cost"] = query.Sum(x => x.cc);
            f["Margin"] = query.Sum(x => x.cm);
            f["MarginP"] = query.Sum(x => x.cs) == 0 ? 0 : (query.Sum(x => x.cm) / query.Sum(x => x.cs)) * 100;

            f["ActiveValue"] = query.Sum(x => x.acs);
            f["ActiveCost"] = query.Sum(x => x.acc);
            f["ActiveMargin"] = query.Sum(x => x.acm);
            f["ActiveMarginP"] = (query.Sum(x => x.acs) == 0 || query.Sum(x => x.acs) == null) ? 0 : (query.Sum(x => x.acm) / query.Sum(x => x.acs)) * 100;

            f["CancelledValue"] = query.Sum(x => x.ls);
            f["CancelledCost"] = query.Sum(x => x.lc);
            f["CancelledMargin"] = query.Sum(x => x.lm);
            f["CancelledMarginP"] = (query.Sum(x => x.ls) == 0 || query.Sum(x => x.ls) == null) ? 0 : (query.Sum(x => x.lm) / query.Sum(x => x.ls)) * 100;

            dt.Rows.Add(f);

            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            #region -- Table Top Header ---

            TableCell cell = new TableCell();
            cell.Text = year;
            cell.HorizontalAlign = HorizontalAlign.Center;
            GridViewRow headerRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            headerRow.Cells.Add(cell);

            TableCell cell2 = new TableCell();
            cell2.Text = "Budget";
            cell2.ColumnSpan = 4;
            cell2.HorizontalAlign = HorizontalAlign.Center;
            cell2.BackColor = Color.Yellow;
            headerRow.Cells.Add(cell2);

            TableCell cell3 = new TableCell();
            cell3.Text = "Confirmed/Contracted/Operated/Closed";
            cell3.ColumnSpan = 4;
            cell3.Wrap = true;
            cell3.HorizontalAlign = HorizontalAlign.Center;
            cell3.BackColor = Color.LightGreen;
            headerRow.Cells.Add(cell3);

            TableCell cell4 = new TableCell();
            cell4.Text = "Active/Potential";
            cell4.ColumnSpan = 4;
            cell4.HorizontalAlign = HorizontalAlign.Center;
            cell4.BackColor = Color.LightBlue;
            headerRow.Cells.Add(cell4);

            TableCell cell5 = new TableCell();
            cell5.Text = "Active/Potential";
            cell5.ColumnSpan = 4;
            cell5.HorizontalAlign = HorizontalAlign.Center;
            cell5.BackColor = Color.Red;
            headerRow.Cells.Add(cell5);

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

            c1.Text = "Month";
            //Budget
            c2.Text = "Target Value";
            c3.Text = "Target Cost";
            c4.Text = "Target Margin";
            c5.Text = "Target Margin%";
            //Confirmed
            c6.Text = "Value";
            c7.Text = "Cost";
            c8.Text = "Margin";
            c9.Text = "Margin%";
            //Active
            c10.Text = "Value";
            c11.Text = "Cost";
            c12.Text = "Margin";
            c13.Text = "Margin%";
            //Cancelled
            c14.Text = "Value";
            c15.Text = "Cost";
            c16.Text = "Margin";
            c17.Text = "Margin%";

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
            lowerRow.Cells.Add(c17);

            gv.Controls[0].Controls.RemoveAt(1);
            gv.Controls[0].Controls.AddAt(1, lowerRow);

            #endregion

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row3 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row4 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Year:";
            cell_1.BackColor = Color.Yellow;
            cell_1.HorizontalAlign = HorizontalAlign.Left;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = year;
            cell_11.ColumnSpan = 16;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "Branch:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = CompanyRepository.BranchName(branch);
            cell_22.ColumnSpan = 16;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "File Handler:";
            cell_3.BackColor = Color.Yellow;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = EmployeeRepository.EmployeeName(empid);
            cell_33.ColumnSpan = 16;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Nature:";
            cell_4.BackColor = Color.Yellow;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = BusinessNatureRepository.NatureName(natid);
            cell_44.ColumnSpan = 16;
            row4.Cells.Add(cell_44);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=TargetPerformanceReport.xls");

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

        //[ActionAuthorize]
        public ActionResult BPRIndex()
        {
            SelectList year = new SelectList(Years(), "Text", "Value");
            ViewBag.Year = year;

            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().Where(x => x.Id != 1 && x.Id != 2).OrderBy(x => x.Name), "Id", "Name");

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
            return View();

        }

        #endregion

    }
}
