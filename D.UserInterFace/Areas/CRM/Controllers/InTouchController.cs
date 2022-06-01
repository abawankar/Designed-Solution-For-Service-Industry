using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class InTouchController : Controller
    {
        //
        // GET: /InTouch/

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult StayInTouch(int id)
        {
            ContactRepository dal = new ContactRepository();
            var contact = dal.GetById(id);

            EmailTemplateRepository e = new EmailTemplateRepository();
            ViewBag.noteTemplate = new SelectList(e.GetAll().Where(x => x.Type == 1), "Id", "TemplateName");

            return View(contact);
        }

        [HttpPost]
        public JsonResult GetTemplate(int id)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            var model = dal.GetAll().Where(x => x.Id == id && x.Type == 1);
            return Json(new { Result = model });
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult SendOneStayInTouch(OneStayInTouchModel model)
        {
            OneStayInTouchRepository dal = new OneStayInTouchRepository();
            model.Date = MyExtension.UAETime();
            model.Emp = User.Identity.Name.ToUpper();
            dal.Insert(model);
            return RedirectToAction("Index", "OneStayInTouch");

        }
    }
}
