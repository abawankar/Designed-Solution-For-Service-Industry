using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class EmailTemplateController : Controller
    {
        //
        // GET: /EmailTemplate/

       
        public ActionResult Index()
        {
            ViewBag.type = new SelectList(new[] { 
            new SelectListItem{Value="0", Text="Mass Email"},
            new SelectListItem{Value="1", Text="Stay In Touch"},
            }, "Value", "Text");

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateTemplate(EmailTemplateModel obj)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            dal.Insert(obj);
            return RedirectToAction("Index", "IndexTemplate");
        }

    }
}
