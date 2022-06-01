using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationServices.Web;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Controllers
{
    [HandleError]
    
    public class CountryController : Controller
    {
        //
        // GET: /Country/

        public ActionResult Index()
        {
            if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR011"))
            {
                return View();
            }
            else
            {
                return View("_unAuthorised");
            }
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                CountryRepository dal = new CountryRepository();
                List<CountryModel> model = new List<CountryModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().Where(x => x.Name.Contains(name)).ToList();
                }
                int count = model.Count;
                List<CountryModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create Country
       
        [HttpPost]
        public JsonResult Create(CountryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR009"))
                {
                    CountryRepository dal = new CountryRepository();
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

        //Update Country details
       
        [HttpPost]
        public JsonResult Update(CountryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                if (User.IsInRole("Admin") || IdentityExtensions.EmpRights.Contains("HR009"))
                {
                    CountryRepository dal = new CountryRepository();
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

        [HttpPost]
        public JsonResult GetCountryOptions()
        {
            try
            {
                CountryRepository dal = new CountryRepository();
                var list = dal.GetAll()
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult ExportData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Id", typeof(int)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));

            GridView gv = new GridView();
            CountryRepository dal = new CountryRepository();
            foreach (var item in dal.GetAll())
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = item.Id;
                dr["Country"] = item.Name;
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Country.xls");

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

        [HttpPost]
        public JsonResult SelectCountryOptions(int clientId = 0)
        {
            CountryRepository dal = new CountryRepository();
            List<CountryModel> model = new List<CountryModel>();
            model.Add(new CountryModel { Id = 0, Name = "* Select *" });
            var data1 = model.Select(c => new { DisplayText = c.Name, Value = c.Id });

            try
            {
                if (clientId == 0)
                {
                    var data = dal.GetAll()
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.Concat(data1).OrderBy(x => x.DisplayText) });
                }
                else
                {
                    ClientRepository cl = new ClientRepository();
                    int id = cl.GetById(clientId).Country.Id;
                    var data = dal.GetAll().Where(x => x.Id == id)
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.Concat(data1).OrderBy(x => x.DisplayText) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}
