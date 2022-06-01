using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    public class HomeController : Controller
    {
        // GET: CRM/Home
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}