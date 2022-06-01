using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    [HandleError]
    
    public class EnquiryStatusController : Controller
    {
        //
        // GET: /EnquiryStatus/

        //[ActionAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                EnquiryStatusRepository dal = new EnquiryStatusRepository();
                List<EnquiryStatusModel> model = new List<EnquiryStatusModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().Where(x => x.Name.Contains(name)).ToList();
                }
                int count = model.Count;
                List<EnquiryStatusModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new Enquiry Status
       
        [HttpPost]
        public JsonResult Create(EnquiryStatusModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                EnquiryStatusRepository dal = new EnquiryStatusRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update Enquiry Status details
       
        [HttpPost]
        public JsonResult Update(EnquiryStatusModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                EnquiryStatusRepository dal = new EnquiryStatusRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetStatusOptions()
        {
            try
            {
                EnquiryStatusRepository dal = new EnquiryStatusRepository();
                var list = dal.GetAll().OrderBy(x => x.Name)
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SelectStatusOptions()
        {
            List<EnquiryStatusModel> model = new List<EnquiryStatusModel>();
            model.Add(new EnquiryStatusModel { Id = 0, Name = "* Select *" });
            var data = model.Select(c => new { DisplayText = c.Name, Value = c.Id });
            try
            {
                EnquiryStatusRepository dal = new EnquiryStatusRepository();
                var list = dal.GetAll().OrderBy(x => x.Name)
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list.Concat(data).OrderBy(x => x.DisplayText) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetStatus()
        {
            try
            {
                EnquiryStatusRepository dal = new EnquiryStatusRepository();
                var list = from m in dal.GetAll()
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}
