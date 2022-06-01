using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class AccountMailLogController : Controller
    {
        //
        // GET: /AccountMailLog/

       
        public ActionResult Index(int accountid = 0)
        {
            ViewBag.accountid = accountid;
            return View();
        }

        [HttpPost]
        public JsonResult List(int accountid = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                AccountMailLogRepository dal = new AccountMailLogRepository();
                List<AccountMailLogModel> model = new List<AccountMailLogModel>();
                model = dal.GetAll().ToList();
                if (accountid != 0)
                {
                    model = model.Where(x => x.AccountId.Id == accountid).ToList();
                }

                int count = model.Count;
                model = model.OrderByDescending(x => x.Id).ToList();
                List<AccountMailLogModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetContactList(int id = 0)
        {
            AccountMailLogRepository dal = new AccountMailLogRepository();
            try
            {
                var data = dal.GetById(id).ContactList.ToList();
                return Json(new { Result = "OK", Records = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult Mailbody(int id)
        {
            AccountMailLogRepository dal = new AccountMailLogRepository();
            AccountMailLogModel bl = dal.GetById(id);
            return PartialView(bl);
        }

       
        [HttpPost]
        public JsonResult Delete(AccountMailLogModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                AccountMailLogRepository dal = new AccountMailLogRepository();
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
