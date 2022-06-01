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

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class LeastProducerController : Controller
    {
        static List<NBOModel> exportList = new List<NBOModel>();
        static List<LeastProducerModel> lowProducer = new List<LeastProducerModel>();
        //
        // GET: /LeastProducer/

       
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
        public JsonResult List(int acttype = 0, string city = null, string country = null, string branch = null, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                NBORepository nbo = new NBORepository();
                var account = acttype == 0 ? dal.GetAll() : dal.GetAll().Where(x => x.ActType.Any(y => y.Id == acttype));
                var date = MyExtension.UAETime().AddMonths(-6);
                var business = nbo.GetAll().ToList();
                business = business.OrderBy(x => x.RequestDate).ToList();
                var data = from a in account
                           join n in business on a.AccountId equals n.ClientName.Id into g
                           from sub in g.DefaultIfEmpty()
                           select new LeastProducerModel
                           {
                               AccountName = a.AccountName,
                               AccountId = a.AccountId,
                               ActId = a.Id,
                               Country = a.Country.Name,
                               City = a.City,
                               Branch = (sub == null ? "" : sub.Branch.Name),
                               Business = (sub == null ? 0 : Convert.ToDouble(sub.ContractValue)),
                               Days = sub == null ? (Convert.ToDateTime(MyExtension.UAETime().ToShortDateString()) - Convert.ToDateTime(Convert.ToDateTime(a.Date).ToShortDateString())).TotalDays : (Convert.ToDateTime(MyExtension.UAETime().ToShortDateString()) - Convert.ToDateTime(sub.RequestDate.ToShortDateString())).TotalDays,
                               Date = Convert.ToDateTime(a.Date),
                               Owner = a.Owner.EmpName
                           };
                var list = from m in data
                           group m by m.AccountName into g
                           select new LeastProducerModel
                           {
                               AccountName = g.Key,
                               AccountId = g.Select(x => x.AccountId).Take(1).SingleOrDefault(),
                               ActId = g.Select(x => x.ActId).Take(1).SingleOrDefault(),
                               Country = g.Select(x => x.Country).Take(1).SingleOrDefault(),
                               City = g.Select(x => x.City).Take(1).SingleOrDefault(),
                               Branch = g.Select(x => x.Branch).Take(1).SingleOrDefault(),
                               Days = g.Min(x => x.Days),
                               Business = g.Sum(x => x.Business),
                               Owner = g.Select(x => x.Owner).Take(1).SingleOrDefault(),
                               Date = g.Select(x => x.Date).Take(1).SingleOrDefault()
                           };

                var leastproducer = list.Where(x => x.Days >= 30).ToList();

                if (!string.IsNullOrEmpty(branch))
                {
                    leastproducer = leastproducer.Where(x => x.Branch == branch).ToList();
                }
                if (!string.IsNullOrEmpty(country))
                {
                    leastproducer = leastproducer.Where(x => x.Country == country).ToList();
                }
                if (!string.IsNullOrEmpty(city))
                {
                    leastproducer = leastproducer.Where(x => x.City.ToUpper() == city).ToList();
                }

                if (name == "90")
                {
                    leastproducer = leastproducer.Where(x => x.Days <= 90 && x.Days > 30).ToList();
                }
                if (name == "30")
                {
                    leastproducer = leastproducer.Where(x => x.Days <= 30).ToList();
                }
                if (name == "6")
                {
                    leastproducer = leastproducer.Where(x => x.Days > 90).ToList();
                }

                lowProducer.Clear();
                lowProducer = leastproducer.ToList();
                int count = leastproducer.Count();
                leastproducer = leastproducer.OrderByDescending(x => x.Days).ToList();
                List<LeastProducerModel> Model1 = leastproducer.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult FileList(int id = 0, string branch = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                List<NBOModel> model = new List<NBOModel>();
                model = dal.GetAccountFiles(id).ToList();
                if (!String.IsNullOrEmpty(branch))
                {
                    model = model.Where(x => x.Branch.Name == branch).ToList();
                }
                exportList.Clear();
                exportList = model.ToList();
                model = model.OrderByDescending(x => x.RequestDate).ToList();
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


       
        public ActionResult ExportLeastProducer()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("AccountName", typeof(string)));
            dt.Columns.Add(new DataColumn("Owner", typeof(string)));
            dt.Columns.Add(new DataColumn("CreationDate", typeof(string)));
            dt.Columns.Add(new DataColumn("Days", typeof(string)));

            GridView gv = new GridView();
            var data = lowProducer.OrderByDescending(x => x.Days).ToList();

            foreach (var item in data)
            {
                DataRow dr = dt.NewRow();
                dr["AccountName"] = item.AccountName;
                dr["Owner"] = item.Owner;
                dr["CreationDate"] = Convert.ToDateTime(item.Date).ToString("dd-MMM-yyyy");
                dr["Days"] = item.Days;
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = "LeastProducer.xls";
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
