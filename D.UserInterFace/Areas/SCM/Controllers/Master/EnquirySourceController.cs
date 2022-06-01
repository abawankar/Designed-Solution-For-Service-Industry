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
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    public class EnquirySourceController : Controller
    {
        //
        // GET: /EnquirySource/

        //[Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                EnquirySourceRepository dal = new EnquirySourceRepository();
                List<EnquirySourceModel> model = new List<EnquirySourceModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().Where(x => x.Name.Contains(name.ToUpper())).ToList();
                }
                int count = model.Count;
                List<EnquirySourceModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new Enquiry Source
        [HttpPost]
        public JsonResult Create(EnquirySourceModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                EnquirySourceRepository dal = new EnquirySourceRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update Enquiry Source details
        [HttpPost]
        public JsonResult Update(EnquirySourceModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                EnquirySourceRepository dal = new EnquirySourceRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetEnqSourceOptions()
        {
            try
            {
                EnquirySourceRepository dal = new EnquirySourceRepository();
                var list = dal.GetAll().Where(x => x.Active == true).OrderBy(x => x.Name)
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SelectEnqSourceOptions()
        {
            List<EnquirySourceModel> model = new List<EnquirySourceModel>();
            model.Add(new EnquirySourceModel { Id = 0, Name = "* Select *" });
            var data = model.Select(c => new { DisplayText = c.Name, Value = c.Id });
            try
            {
                EnquirySourceRepository dal = new EnquirySourceRepository();
                var list = dal.GetAll().Where(x => x.Active == true).OrderBy(x => x.Name)
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list.Concat(data).OrderBy(x => x.DisplayText) });
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
            dt.Columns.Add(new DataColumn("Source", typeof(string)));
            dt.Columns.Add(new DataColumn("DateOfAppointment", typeof(string)));
            dt.Columns.Add(new DataColumn("DateOfTermination", typeof(string)));
            dt.Columns.Add(new DataColumn("RetainerFee", typeof(string)));
            dt.Columns.Add(new DataColumn("CommLeisure", typeof(string)));
            dt.Columns.Add(new DataColumn("CommMice", typeof(string)));
            dt.Columns.Add(new DataColumn("Active", typeof(string)));

            GridView gv = new GridView();
            EnquirySourceRepository dal = new EnquirySourceRepository();
            foreach (var item in dal.GetAll())
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = item.Id;
                dr["Source"] = item.Name;
                dr["DateOfAppointment"] = item.AppointmentDate != null ? Convert.ToDateTime(item.AppointmentDate).ToString("dd/MM/yyyy") : "";
                dr["DateOfTermination"] = item.TerminationDate != null ? Convert.ToDateTime(item.TerminationDate).ToString("dd/MM/yyyy") : "";
                dr["RetainerFee"] = item.RetainerFee;
                dr["CommLeisure"] = item.CommLeisure;
                dr["CommMice"] = item.CommMice;
                if (item.Active == true)
                    dr["Active"] = "Yes";
                else
                    dr["Active"] = "No";
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=EnquirySource.xls");

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
