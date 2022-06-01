using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.SCM.Models.Transaction;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    [HandleError]
    
    public class PayableController : Controller
    {
        //
        // GET: /Outgoing/

       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(int nboid = 0)
        {
            try
            {
                PayableRepository dal = new PayableRepository();
                List<PayableModel> model = new List<PayableModel>();
                model = dal.GetPayable(nboid).ToList();
                int count = model.Count;
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Create(PayableModel model, int nboid)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                //}
                PayableRepository dal = new PayableRepository();
                dal.InsertPayable(model, nboid);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Update(PayableModel model)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                //}
                PayableRepository dal = new PayableRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Delete(PayableModel model)
        {
            try
            {
                PayableRepository dal = new PayableRepository();
                dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }
    }
}
