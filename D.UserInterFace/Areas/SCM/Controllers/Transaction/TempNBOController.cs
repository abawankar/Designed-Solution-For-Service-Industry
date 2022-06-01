using D.UserInterFace.Areas.SCM.Models.Master;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Models.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    public class TempNBOController : Controller
    {
        static List<TempNBOModel> exportModel = new List<TempNBOModel>();
        //
        // GET: /TempNBO/

        public ActionResult Index()
        {
            EmployeeRepository e = new EmployeeRepository();
            CompanyRepository c = new CompanyRepository();

            ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");
            ViewBag.Branch = new SelectList(c.BranchList(), "Id", "Name");

            BusinessNatureRepository n = new BusinessNatureRepository();
            ViewBag.Nature = new SelectList(n.GetAll().Where(x => x.Id != 6).OrderBy(x => x.Name), "Id", "Name");

            NatureGroupRepository ng = new NatureGroupRepository();
            ViewBag.NatureGroup = new SelectList(ng.GetAll().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        [HttpPost]
        public JsonResult List(string dateFrom = null, string dateTo = null, int gnature = 0, int source = 0, string status = null, string col = null, int branch = 0, int empId = 0, int nature = 0, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NatureGroupRepository natdal = new NatureGroupRepository();

                TempNBORepository dal = new TempNBORepository();
                List<TempNBOModel> model = new List<TempNBOModel>();
                model = dal.GetAll().ToList();
                if (!string.IsNullOrEmpty(col))
                {
                    switch (col)
                    {
                        case "Emp":
                            {
                                if (empId != 0)
                                    model = model.Where(x => x.EmpId == empId).ToList();
                            }
                            break;
                        case "nature":
                            {
                                if (nature != 0)
                                    model = model.Where(x => x.Nature.Id == nature).ToList();
                            }
                            break;
                        case "gnature":
                            {
                                if (gnature != 0)
                                {
                                    var natureid = from c in natdal.GetById(gnature).NatureName
                                                   select c.Id;
                                    model = model.Where(x => natureid.Contains(x.Nature.Id)).ToList();
                                }
                            }
                            break;
                        case "branch":
                            {
                                if (branch != 0)
                                    model = model.Where(x => x.Branch.Id == branch).ToList();
                            }
                            break;
                        case "st":
                            {
                                string[] id = status.Split(',');
                                model = model.Where(x => id.Contains(x.Status.Id.ToString())).ToList();
                            }
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
                            {
                                if (branch != 0)
                                    model = model.Where(x => x.Branch.Id == branch).ToList();

                                if ((dateFrom != null && dateTo != null) && (dateFrom != "" && dateTo != ""))
                                {
                                    model = model.Where(x => x.CheckinDate >= Convert.ToDateTime(dateFrom) && x.CheckinDate <= Convert.ToDateTime(dateTo)).ToList();
                                }

                                if (empId == 0 && nature != 0)
                                {
                                    model = model.Where(x => x.Nature.Id == nature).ToList();
                                }
                                if (empId != 0 && nature == 0)
                                    model = model.Where(x => x.FileHandler.Id == empId).ToList();

                                if (empId != 0 && nature != 0)
                                {
                                    model = model.Where(x => x.FileHandler.Id == empId && x.Nature.Id == nature).ToList();
                                }

                                if (gnature != 0)
                                {
                                    var natureid = from c in natdal.GetById(gnature).NatureName
                                                   select c.Id;
                                    model = model.Where(x => natureid.Contains(x.Nature.Id)).ToList();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                int count = model.Count;
                model = model.OrderByDescending(x => x.CheckinDate).ToList();
                exportModel.Clear();
                exportModel = model.OrderBy(x => x.CheckinDate).ToList();
                List<TempNBOModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
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
            dt.Columns.Add(new DataColumn("ContractValue", typeof(double)));
            dt.Columns.Add(new DataColumn("ContractCost", typeof(double)));
            dt.Columns.Add(new DataColumn("Margin", typeof(double)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));

            GridView gv = new GridView();

            var data = exportModel.ToList();
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
                    dr["EventName"] = item.ClientCountry.Name;
                    dr["Pax"] = item.PaxNo;
                    dr["CheckIn"] = Convert.ToDateTime(item.CheckinDate).ToString("dd-MM-yyyy");
                    dr["CheckOut"] = Convert.ToDateTime(item.CheckOutDate).ToString("dd-MM-yyyy");
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
            string fileName = "CopyOfConfirmed&ContractedFiles.xls";
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

        public List<TempNBOModel> GetAll(int branch = 0, int empId = 0, int nature = 0)
        {
            TempNBORepository dal = new TempNBORepository();
            List<TempNBOModel> model = new List<TempNBOModel>();
            model = dal.GetAll().ToList();


            if (branch != 0 && empId == 0 && nature == 0)
                model = model.Where(x => x.Branch.Id == branch).ToList();
            if (branch != 0 && empId == 0 && nature != 0)
                model = model.Where(x => x.Branch.Id == branch && x.Nature.Id == nature).ToList();
            if (branch == 0 && empId != 0 && nature != 0)
                model = model.Where(x => x.FileHandler.Id == empId && x.Nature.Id == nature).ToList();
            if (branch != 0 && empId != 0 && nature == 0)
                model = model.Where(x => x.Branch.Id == branch && x.FileHandler.Id == empId).ToList();

            return model;

        }

    }
}
