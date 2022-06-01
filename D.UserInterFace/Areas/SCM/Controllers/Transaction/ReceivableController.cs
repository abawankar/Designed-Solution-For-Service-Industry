using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.SCM.Models.Transaction;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    [HandleError]
    
    public class ReceivableController : Controller
    {
        //
        // GET: /Incoming/

       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(int nboid = 0)
        {
            try
            {
                ReceivableRepository dal = new ReceivableRepository();
                List<ReceivableModel> model = new List<ReceivableModel>();
                model = dal.GetReceivable(nboid).ToList();
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Create(ReceivableModel model, int nboid)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                //}
                ReceivableRepository dal = new ReceivableRepository();
                dal.InsertReceivable(model, nboid);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Update(ReceivableModel model)
        {
            try
            {
                ReceivableRepository dal = new ReceivableRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Delete(ReceivableModel model)
        {
            try
            {
                ReceivableRepository dal = new ReceivableRepository();
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
