using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationServices.Web;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class TopProducerController : Controller
    {
        static List<NBOModel> exportList = new List<NBOModel>();
        static List<TopProducerModel> topList = new List<TopProducerModel>();

        //
        // GET: /TopProducer/

       
        public ActionResult Index()
        {
            AccountTypeRepository type = new AccountTypeRepository();
            ViewBag.acttype = new SelectList(type.GetAll(), "Id", "Name");

            CompanyRepository c = new CompanyRepository();
            ViewBag.Branch = new SelectList(c.BranchList(), "Name", "Name");

            CountryRepository ct = new CountryRepository();
            ViewBag.Country = new SelectList(ct.GetAll().OrderBy(x => x.Name), "Name", "Name");

            NewAccountRepository act = new NewAccountRepository();
            var city = from m in act.GetAll()
                       select new { City = m.City.ToUpperIgnoreNull() };
            ViewBag.city = new SelectList(city.Distinct().OrderBy(x => x.City), "City", "City");
            return View();
        }

        [HttpPost]
        public JsonResult List(int acttype = 0, string city = null, string country = null, string branch = null, string dateFrom = null, string dateTo = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                NBORepository nbo = new NBORepository();
                var account = acttype == 0 ? dal.GetAll() : dal.GetAll().Where(x => x.ActType.Any(y => y.Id == acttype));
                var business = nbo.GetAll().Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).ToList();
                business = business.Where(x => x.ContractValue != 0).ToList();
                if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
                {
                    business = business.Where(x => x.CheckinDate >= Convert.ToDateTime(dateFrom) && x.CheckinDate <= Convert.ToDateTime(dateTo)).ToList();
                }
                var data = from a in account
                           join n in business on a.AccountId equals n.ClientName.Id into g
                           from sub in g.DefaultIfEmpty()
                           select new
                           {
                               AccountName = a.AccountName,
                               AccountId = a.AccountId,
                               fils = 1,
                               Country = a.Country.Name,
                               City = a.City,
                               Branch = (sub == null ? "" : sub.Branch.Name),
                               ContractValue = (sub == null ? 0 : sub.ContractValue),
                               ContractCost = (sub == null ? 0 : sub.ContractCost),
                               Margin = (sub == null ? 0 : (sub.ContractValue - sub.ContractCost)),
                               Pax = (sub == null ? 0 : sub.PaxNo)
                           };
                var topProducer = from m in data.Where(x => x.ContractValue != 0)
                                  group m by m.AccountName into g
                                  select new TopProducerModel
                                  {
                                      Id = 1,
                                      AccountName = g.Key,
                                      AccountId = g.Select(x => x.AccountId).Take(1).Single(),
                                      Country = g.Select(x => x.Country).Take(1).Single(),
                                      City = g.Select(x => x.City).Take(1).Single(),
                                      Branch = g.Select(x => x.Branch).Take(1).Single(),
                                      ContractValue = Convert.ToDouble(g.Sum(x => x.ContractValue)),
                                      ContractCost = Convert.ToDouble(g.Sum(x => x.ContractCost)),
                                      Margin = Convert.ToDouble(g.Sum(x => x.Margin)),
                                      MarginP = (Convert.ToDouble(g.Sum(x => x.Margin)) / Convert.ToDouble(g.Sum(x => x.ContractValue))) * 100,
                                      TotalFiles = g.Sum(x => x.fils),
                                      TotalPax = g.Sum(x => x.Pax)
                                  };

                if (!string.IsNullOrEmpty(branch))
                {
                    topProducer = topProducer.Where(x => x.Branch == branch).ToList();
                }
                if (!string.IsNullOrEmpty(country))
                {
                    topProducer = topProducer.Where(x => x.Country == country).ToList();
                }
                if (!string.IsNullOrEmpty(city))
                {
                    topProducer = topProducer.Where(x => x.City.ToUpper() == city).ToList();
                }

                int count = topProducer.Count();
                topList.Clear();
                topList = topProducer.ToList();
                topProducer = topProducer.OrderByDescending(x => x.ContractValue).ToList();
                List<TopProducerModel> Model1 = topProducer.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult FileList(int id = 0, string branch = null, string dateFrom = null, string dateTo = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                List<NBOModel> model = new List<NBOModel>();
                model = dal.GetAccountFiles(id).Where(x => x.ContractValue != 0).ToList();
                if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
                {
                    model = model.Where(x => x.CheckinDate >= Convert.ToDateTime(dateFrom) && x.CheckinDate <= Convert.ToDateTime(dateTo)).ToList();
                }
                if (!String.IsNullOrEmpty(branch))
                {
                    model = model.Where(x => x.Branch.Name == branch).ToList();
                }
                model = model.Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).ToList();

                exportList.Clear();
                exportList = model.ToList();
                model = model.OrderByDescending(x => x.CheckinDate).ToList();
                int count = model.Count;

                List<NBOModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        public ActionResult ExportFiles()
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
            var data = exportList.OrderBy(x => x.RequestDate).ToList();

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

       
        public ActionResult ExportTopProducer()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("AccountName", typeof(string)));
            dt.Columns.Add(new DataColumn("ContractValue", typeof(string)));
            dt.Columns.Add(new DataColumn("ContractCost", typeof(string)));
            dt.Columns.Add(new DataColumn("Margin", typeof(string)));
            dt.Columns.Add(new DataColumn("Margin%", typeof(string)));
            dt.Columns.Add(new DataColumn("TotalFiles", typeof(string)));
            dt.Columns.Add(new DataColumn("TotalPax", typeof(string)));

            GridView gv = new GridView();
            var data = topList.OrderBy(x => x.ContractValue).ToList();

            foreach (var item in data)
            {
                DataRow dr = dt.NewRow();
                dr["AccountName"] = item.AccountName;
                dr["ContractValue"] = item.ContractValue.ToString("##,##0,#0");
                dr["ContractCost"] = item.ContractCost.ToString("##,##0,#0");
                dr["Margin"] = item.Margin.ToString("##,##0,#0");
                dr["Margin%"] = item.MarginP.ToString("##,##0,#0");
                dr["TotalFiles"] = item.TotalFiles;
                dr["TotalPax"] = item.TotalPax;
                dt.Rows.Add(dr);
            }
            DataRow GrandTotal = dt.NewRow();
            GrandTotal["AccountName"] = "Grand Total";
            GrandTotal["ContractValue"] = Convert.ToDouble(data.Sum(x => x.ContractValue)).ToString("##,###.00");
            GrandTotal["ContractCost"] = Convert.ToDouble(data.Sum(x => x.ContractCost)).ToString("##,###.00");
            GrandTotal["Margin"] = Convert.ToDouble(data.Sum(x => x.Margin)).ToString("##,###.00");
            GrandTotal["Margin%"] = ((data.Sum(x => x.Margin) / data.Sum(x => x.ContractValue)) * 100).ToString("##.00");
            GrandTotal["TotalFiles"] = Convert.ToDouble(data.Sum(x => x.TotalFiles)).ToString("##,###.00");
            GrandTotal["TotalPax"] = Convert.ToDouble(data.Sum(x => x.TotalPax)).ToString("##,###.00");
            dt.Rows.Add(GrandTotal);

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = "TopProducer.xls";
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
