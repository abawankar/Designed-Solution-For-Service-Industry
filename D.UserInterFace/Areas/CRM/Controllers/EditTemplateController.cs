using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class EditTemplateController : Controller
    {
        //
        // GET: /EditTemplate/

       
        public ActionResult Index(int id)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            EmailTemplateModel bl = dal.GetById(id);

            ViewBag.type = new SelectList(new[] { 
            new SelectListItem{Value="0", Text="Mass Email"},
            new SelectListItem{Value="1", Text="Stay In Touch"},
            }, "Value", "Text");

            return View(bl);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditTemplate(EmailTemplateModel obj)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            dal.Edit(obj);
            return RedirectToAction("Index", "IndexTemplate");
        }

    }
}
