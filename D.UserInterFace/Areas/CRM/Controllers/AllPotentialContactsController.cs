using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class AllPotentialContactsController : Controller
    {
        static List<PotentialContactModel> contactlist = new List<PotentialContactModel>();
        //
        // GET: /AllPotentialContacts/

       
        public ActionResult Index()
        {
            CountryRepository c = new CountryRepository();

            ViewBag.Country = new SelectList(c.GetAll().OrderBy(x => x.Name), "Id", "Name");

            return View();
        }

        [HttpPost]
        public JsonResult ContactList(int groupid = 0, int countryid = 0, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            PotentialContactRepository dal = new PotentialContactRepository();
            try
            {
                List<PotentialContactModel> model = new List<PotentialContactModel>();
                model = dal.GetAll().ToList();
                if (!String.IsNullOrEmpty(name))
                {
                    if (countryid != 0)
                    {
                        model = model.Where(x => x.Country.Id == countryid).ToList();
                        model = model.Where(x => x.FirstName.StartsWith(name) ||
                            x.AccountName.StartsWith(name)).ToList();
                    }
                    else
                    {
                        if (name != "0")
                        {
                            model = model.Where(x => x.FirstName.StartsWith(name) ||
                               x.AccountName.StartsWith(name)).ToList();
                        }
                    }
                }
                if (countryid != 0)
                {
                    model = model.Where(x => x.Country.Id == countryid).ToList();
                }

                int count = model.Count;
                contactlist.Clear();
                contactlist = model;
                model = model.OrderBy(x => x.FirstName).ToList();
                List<PotentialContactModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public ActionResult ExportData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Title", typeof(string)));
            dt.Columns.Add(new DataColumn("FirstName", typeof(string)));
            dt.Columns.Add(new DataColumn("MiddleName", typeof(string)));
            dt.Columns.Add(new DataColumn("LastName", typeof(string)));
            dt.Columns.Add(new DataColumn("Email", typeof(string)));
            dt.Columns.Add(new DataColumn("Phone", typeof(string)));
            dt.Columns.Add(new DataColumn("Mobile", typeof(string)));
            dt.Columns.Add(new DataColumn("Fax", typeof(string)));
            dt.Columns.Add(new DataColumn("Department", typeof(string)));
            dt.Columns.Add(new DataColumn("Street", typeof(string)));
            dt.Columns.Add(new DataColumn("City", typeof(string)));
            dt.Columns.Add(new DataColumn("StateProvince", typeof(string)));
            dt.Columns.Add(new DataColumn("ZipPostalCode", typeof(string)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));
            dt.Columns.Add(new DataColumn("AccountName", typeof(string)));

            GridView gv = new GridView();

            foreach (var item in contactlist.OrderBy(x => x.FirstName))
            {
                DataRow dr = dt.NewRow();
                dr["Title"] = item.Title;
                dr["FirstName"] = item.FirstName;
                dr["MiddleName"] = item.MiddleName;
                dr["LastName"] = item.LastName;
                dr["Email"] = item.Email;
                dr["Phone"] = item.Phone;
                dr["Mobile"] = item.Mobile;
                dr["Fax"] = item.Fax;
                dr["Department"] = item.Department;
                dr["Country"] = item.Country.Name;
                dr["Street"] = item.Street;
                dr["City"] = item.City;
                dr["StateProvince"] = item.StateProvince;
                dr["ZipPostalCode"] = item.ZipPostalCode;
                dr["AccountName"] = item.AccountName;
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = "PotentialContacts.xls";
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
