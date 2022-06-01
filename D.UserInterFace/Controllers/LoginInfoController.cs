using D.UserInterFace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace D.UserInterFace.Controllers
{
    public class LoginInfoController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: LoginInfo
        public ActionResult Index()
        {
            ViewBag.logintype = new SelectList(new[] {
                new SelectListItem{Value="Emp", Text="Employee"}
                }, "Value", "Text");
            return View();
        }

        [HttpPost]
        public JsonResult List(string name = null, string type = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var users = context.Users.ToList();
                LoginInfoRepository dal = new LoginInfoRepository();
                List<LoginInfoModel> model = new List<LoginInfoModel>();
                model = dal.GetAll().ToList();
                if (!string.IsNullOrEmpty(type))
                {
                    switch (type)
                    {
                        case "Emp":
                            {
                                model = model.ToList();
                            }
                            break;
                        
                        default:
                            break;
                    }

                }

                var data = from m in model
                           join u in users on m.UserName equals u.UserName into g
                           from mu in g.DefaultIfEmpty()
                           select new LoginInfoModel
                           {
                               Id = m.Id,
                               Date = m.Date,
                               Time = m.Time,
                               UserName = m.UserName,
                               Name = (mu == null ? string.Empty : mu.FirstName + " " + mu.LastName)
                           };

                if (!string.IsNullOrEmpty(name))
                {
                    data = data.Where(x => (x.Name + " " + x.UserName).ToLower().Contains(name.ToLower())).ToList();
                }

                List<LoginInfoModel> Model1 = data.OrderByDescending(x => x.Id).ToList();
                int count = Model1.Count;
                List<LoginInfoModel> Model2 = Model1.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model2, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult delete(LoginInfoModel model)
        {
            try
            {
                LoginInfoRepository dal = new LoginInfoRepository();
                var data = dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }
    }
}