using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class MassEmailingController : Controller
    {
        //
        // GET: /MassEmailing/


       
        public ActionResult Index()
        {
            ViewBag.status = new SelectList(new[] {
                new SelectListItem { Text = "Pending", Value = "0"},
                new SelectListItem { Text = "Successful", Value = "1" },
                new SelectListItem { Text = "Cancelled", Value = "2" }
            }, "Value", "Text");
            return View();
        }

        [HttpPost]
        public JsonResult ContactList(string contType = null, string actType = null, string actCat = null, string type = null, string contgp = null, string city = null, string ctryid = null, string acname = null, string first = null, string last = null, int jtStartIndex = 0, int jtPageSize = 0)
        {
            NewAccountRepository ac = new NewAccountRepository();
            ContactGroupRepository dal = new ContactGroupRepository();
            ContactRepository ct = new ContactRepository();
            try
            {
                List<ContactModel> list = new List<ContactModel>();
                List<NewAccountModel> act = new List<NewAccountModel>();
                if (!string.IsNullOrEmpty(contgp))
                {
                    string[] idlist = contgp.Split(',');
                    List<ContactModel> ctlist = new List<ContactModel>();
                    foreach (var item in idlist)
                    {

                        ctlist = ctlist.Concat(ct.GetByGroupId(System.Convert.ToInt32(item)).ToList()).ToList();
                    }
                    list = ctlist.ToList();
                }
                else
                {
                    //act = ac.GetAll().ToList();
                    if (!string.IsNullOrEmpty(type))
                    {
                        act = ac.GetAll().Where(x => x.Type == type).ToList();
                    }
                    if (!string.IsNullOrEmpty(actCat))
                    {
                        string[] idlist = actCat.Split(',');
                        if (act.Count == 0)
                        {
                            List<NewAccountModel> actdata = new List<NewAccountModel>();
                            foreach (var ids in idlist)
                            {
                                actdata = actdata.Concat(ac.GetByIndustryid(Convert.ToInt32(ids))).ToList();
                            }
                            act = actdata.ToList();
                        }
                        else
                        {
                            act = act.Where(x => idlist.Contains(x.Industry.Id.ToString())).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(actType))
                    {
                        string[] idlist = actType.Split(',');
                        if (act.Count == 0)
                        {
                            act = ac.GetAll().Where(x => x.ActType.Any(y => idlist.Contains(y.Id.ToString()))).ToList();
                        }
                        else
                        {
                            act = act.Where(x => x.ActType.Any(y => idlist.Contains(y.Id.ToString()))).ToList();
                        }

                    }
                    if (!string.IsNullOrEmpty(acname))
                    {
                        string[] idlist = acname.Split(',');
                        if (act.Count == 0)
                        {
                            List<NewAccountModel> actdata = new List<NewAccountModel>();
                            foreach (var ids in idlist)
                            {
                                actdata = actdata.Concat(ac.GetIdList(Convert.ToInt32(ids))).ToList();
                            }
                            act = actdata.ToList();
                        }
                        else
                        {
                            act = act.Where(x => idlist.Contains(x.Id.ToString())).ToList();
                        }

                    }

                    list = ac.GetContactModel(act).ToList();

                    if (list.Count == 0 && string.IsNullOrEmpty(acname) && string.IsNullOrEmpty(actType) && string.IsNullOrEmpty(actCat) && string.IsNullOrEmpty(type))
                    {
                        list = ct.GetAll().ToList();
                    }

                    list = list.Where(x => x.EmailOpt == false).ToList();

                    if (!string.IsNullOrEmpty(ctryid))
                    {
                        string[] idlist = ctryid.Split(',');
                        list = list.Where(x => x.Country.Id.ToString() != null && idlist.Contains(x.Country.Id.ToString())).ToList();
                    }

                    if (!string.IsNullOrEmpty(city))
                    {
                        string[] idlist = city.Split(',');
                        if (city == "0")
                        {

                        }
                        else
                        {
                            list = list.Where(x => idlist.Contains(x.City.ToUpper())).ToList();
                        }

                    }
                    if (!string.IsNullOrEmpty(contType))
                    {
                        string[] idlist = contType.Split(',');
                        list = list.Where(x => x.ContType.Any(y => idlist.Contains(y.Id.ToString()))).ToList();
                    }
                    if (!string.IsNullOrEmpty(first))
                    {
                        list = list.Where(x => x.FirstName.ToUpper().StartsWith(first.ToUpper())).ToList();
                    }
                    if (!string.IsNullOrEmpty(last))
                    {
                        list = list.Where(x => x.LastName.ToUpper().StartsWith(last.ToUpper())).ToList();
                    }
                }
                int count = list.Count;
                List<ContactModel> Model1 = list.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult CreateMassEmailing()
        {
            ViewBag.timezone = new SelectList(GetTimeZone(), "Value", "Text");

            EmailTemplateRepository e = new EmailTemplateRepository();
            ViewBag.mailtemplate = new SelectList(e.GetAll().Where(x => x.Type == 0), "Id", "TemplateName");

            ViewBag.type = new SelectList(new[] { 
                    new SelectListItem{Text="Client", Value="Client"},
                    new SelectListItem{Text="Supplier", Value="Supplier"},
                }, "Text", "Value");

            return View();

        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                MassEmailingRepository dal = new MassEmailingRepository();
                List<MassEmailingModel> model = new List<MassEmailingModel>();
                model = dal.GetAll().ToList();
                if (!String.IsNullOrEmpty(name))
                {
                    model = model.Where(x => x.Status == Convert.ToInt32(name)).ToList();
                }
                var data = from m in model.ToList()
                           select new MassEmailingModel
                           {
                               Id = m.Id,
                               Name = m.Name,
                               EmpId = m.EmpId,
                               MailTempId = m.MailTempId,
                               Schedule = m.Schedule,
                               Status = m.Status
                           };
                int count = data.Count();
                data = data.OrderByDescending(x => x.Schedule).ThenByDescending(x => x.Id).ToList();
                List<MassEmailingModel> Model1 = data.Skip(jtStartIndex).Take(jtPageSize).ToList();

                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public ActionResult Create(MassEmailingModel model)
        {
            MassEmailingRepository dal = new MassEmailingRepository();
            EmployeeRepository e = new EmployeeRepository();
            model.ServerTime = MyExtension.UAETime();
            model.UserName = User.Identity.Name;
            model.EmpId = e.GetByName(User.Identity.Name.ToUpper());
            model.filelocation = Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")));
            dal.Insert(model);

            return RedirectToAction("Index");
        }

        public List<SelectListItem> GetTimeZone()
        {
            var timezones = new List<SelectListItem> { 
new SelectListItem() { Value="", Text="Select timezone...", Selected = false },
new SelectListItem() { Value="Morocco Standard Time", Text="(GMT) Casablanca", Selected = false },
new SelectListItem() { Value="GMT Standard Time", Text="(GMT) Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London", Selected = false },
new SelectListItem() { Value="Greenwich Standard Time", Text="(GMT) Monrovia, Reykjavik", Selected = false },
new SelectListItem() { Value="W. Europe Standard Time", Text="(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna", Selected = false },
new SelectListItem() { Value="Central Europe Standard Time", Text="(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague", Selected = false },
new SelectListItem() { Value="Romance Standard Time", Text="(GMT+01:00) Brussels, Copenhagen, Madrid, Paris", Selected = false },
new SelectListItem() { Value="Central European Standard Time", Text="(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb", Selected = false },
new SelectListItem() { Value="W. Central Africa Standard Time", Text="(GMT+01:00) West Central Africa", Selected = false },
new SelectListItem() { Value="Jordan Standard Time", Text="(GMT+02:00) Amman", Selected = false },
new SelectListItem() { Value="GTB Standard Time", Text="(GMT+02:00) Athens, Bucharest, Istanbul", Selected = false },
new SelectListItem() { Value="Middle East Standard Time", Text="(GMT+02:00) Beirut", Selected = false },
new SelectListItem() { Value="Egypt Standard Time", Text="(GMT+02:00) Cairo", Selected = false },
new SelectListItem() { Value="South Africa Standard Time", Text="(GMT+02:00) Harare, Pretoria", Selected = false },
new SelectListItem() { Value="FLE Standard Time", Text="(GMT+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius", Selected = false },
new SelectListItem() { Value="Israel Standard Time", Text="(GMT+02:00) Jerusalem", Selected = false },
new SelectListItem() { Value="E. Europe Standard Time", Text="(GMT+02:00) Minsk", Selected = false },
new SelectListItem() { Value="Namibia Standard Time", Text="(GMT+02:00) Windhoek", Selected = false },
new SelectListItem() { Value="Arabic Standard Time", Text="(GMT+03:00) Baghdad", Selected = false },
new SelectListItem() { Value="Arab Standard Time", Text="(GMT+03:00) Kuwait, Riyadh", Selected = false },
new SelectListItem() { Value="Russian Standard Time", Text="(GMT+03:00) Moscow, St. Petersburg, Volgograd", Selected = false },
new SelectListItem() { Value="E. Africa Standard Time", Text="(GMT+03:00) Nairobi", Selected = false },
new SelectListItem() { Value="Georgian Standard Time", Text="(GMT+03:00) Tbilisi", Selected = false },
new SelectListItem() { Value="Iran Standard Time", Text="(GMT+03:30) Tehran", Selected = false },
new SelectListItem() { Value="Arabian Standard Time", Text="(GMT+04:00) Abu Dhabi, Muscat", Selected = false },
new SelectListItem() { Value="Azerbaijan Standard Time", Text="(GMT+04:00) Baku", Selected = false },
new SelectListItem() { Value="Mauritius Standard Time", Text="(GMT+04:00) Port Louis", Selected = false },
new SelectListItem() { Value="Caucasus Standard Time", Text="(GMT+04:00) Yerevan", Selected = false },
new SelectListItem() { Value="Afghanistan Standard Time", Text="(GMT+04:30) Kabul", Selected = false },
new SelectListItem() { Value="Ekaterinburg Standard Time", Text="(GMT+05:00) Ekaterinburg", Selected = false },
new SelectListItem() { Value="Pakistan Standard Time", Text="(GMT+05:00) Islamabad, Karachi", Selected = false },
new SelectListItem() { Value="West Asia Standard Time", Text="(GMT+05:00) Tashkent", Selected = false },
new SelectListItem() { Value="India Standard Time", Text="(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi", Selected = false },
new SelectListItem() { Value="Sri Lanka Standard Time", Text="(GMT+05:30) Sri Jayawardenepura", Selected = false },
new SelectListItem() { Value="Nepal Standard Time", Text="(GMT+05:45) Kathmandu", Selected = false },
new SelectListItem() { Value="N. Central Asia Standard Time", Text="(GMT+06:00) Almaty, Novosibirsk", Selected = false },
new SelectListItem() { Value="Central Asia Standard Time", Text="(GMT+06:00) Astana, Dhaka", Selected = false },
new SelectListItem() { Value="Myanmar Standard Time", Text="(GMT+06:30) Yangon (Rangoon)", Selected = false },
new SelectListItem() { Value="SE Asia Standard Time", Text="(GMT+07:00) Bangkok, Hanoi, Jakarta", Selected = false },
new SelectListItem() { Value="North Asia Standard Time", Text="(GMT+07:00) Krasnoyarsk", Selected = false },
new SelectListItem() { Value="China Standard Time", Text="(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi", Selected = false },
new SelectListItem() { Value="North Asia East Standard Time", Text="(GMT+08:00) Irkutsk, Ulaan Bataar", Selected = false },
new SelectListItem() { Value="Singapore Standard Time", Text="(GMT+08:00) Kuala Lumpur, Singapore", Selected = false },
new SelectListItem() { Value="W. Australia Standard Time", Text="(GMT+08:00) Perth", Selected = false },
new SelectListItem() { Value="Taipei Standard Time", Text="(GMT+08:00) Taipei", Selected = false },
new SelectListItem() { Value="Tokyo Standard Time", Text="(GMT+09:00) Osaka, Sapporo, Tokyo", Selected = false },
new SelectListItem() { Value="Korea Standard Time", Text="(GMT+09:00) Seoul", Selected = false },
new SelectListItem() { Value="Yakutsk Standard Time", Text="(GMT+09:00) Yakutsk", Selected = false },
new SelectListItem() { Value="Cen. Australia Standard Time", Text="(GMT+09:30) Adelaide", Selected = false },
new SelectListItem() { Value="AUS Central Standard Time", Text="(GMT+09:30) Darwin", Selected = false },
new SelectListItem() { Value="E. Australia Standard Time", Text="(GMT+10:00) Brisbane", Selected = false },
new SelectListItem() { Value="AUS Eastern Standard Time", Text="(GMT+10:00) Canberra, Melbourne, Sydney", Selected = false },
new SelectListItem() { Value="West Pacific Standard Time", Text="(GMT+10:00) Guam, Port Moresby", Selected = false },
new SelectListItem() { Value="Tasmania Standard Time", Text="(GMT+10:00) Hobart", Selected = false },
new SelectListItem() { Value="Vladivostok Standard Time", Text="(GMT+10:00) Vladivostok", Selected = false },
new SelectListItem() { Value="Central Pacific Standard Time", Text="(GMT+11:00) Magadan, Solomon Is., New Caledonia", Selected = false },
new SelectListItem() { Value="New Zealand Standard Time", Text="(GMT+12:00) Auckland, Wellington", Selected = false },
new SelectListItem() { Value="Fiji Standard Time", Text="(GMT+12:00) Fiji, Kamchatka, Marshall Is.", Selected = false },
new SelectListItem() { Value="Tonga Standard Time", Text="(GMT+13:00) Nuku'alofa", Selected = false },
new SelectListItem() { Value="Azores Standard Time", Text="(GMT-01:00) Azores", Selected = false },
new SelectListItem() { Value="Cape Verde Standard Time", Text="(GMT-01:00) Cape Verde Is.", Selected = false },
new SelectListItem() { Value="Mid-Atlantic Standard Time", Text="(GMT-02:00) Mid-Atlantic", Selected = false },
new SelectListItem() { Value="E. South America Standard Time", Text="(GMT-03:00) Brasilia", Selected = false },
new SelectListItem() { Value="Argentina Standard Time", Text="(GMT-03:00) Buenos Aires", Selected = false },
new SelectListItem() { Value="SA Eastern Standard Time", Text="(GMT-03:00) Georgetown", Selected = false },
new SelectListItem() { Value="Greenland Standard Time", Text="(GMT-03:00) Greenland", Selected = false },
new SelectListItem() { Value="Montevideo Standard Time", Text="(GMT-03:00) Montevideo", Selected = false },
new SelectListItem() { Value="Newfoundland Standard Time", Text="(GMT-03:30) Newfoundland", Selected = false },
new SelectListItem() { Value="Atlantic Standard Time", Text="(GMT-04:00) Atlantic Time (Canada)", Selected = false },
new SelectListItem() { Value="SA Western Standard Time", Text="(GMT-04:00) La Paz", Selected = false },
new SelectListItem() { Value="Central Brazilian Standard Time", Text="(GMT-04:00) Manaus", Selected = false },
new SelectListItem() { Value="Pacific SA Standard Time", Text="(GMT-04:00) Santiago", Selected = false },
new SelectListItem() { Value="Venezuela Standard Time", Text="(GMT-04:30) Caracas", Selected = false },
new SelectListItem() { Value="SA Pacific Standard Time", Text="(GMT-05:00) Bogota, Lima, Quito, Rio Branco", Selected = false },
new SelectListItem() { Value="Eastern Standard Time", Text="(GMT-05:00) Eastern Time (US & Canada)", Selected = false },
new SelectListItem() { Value="US Eastern Standard Time", Text="(GMT-05:00) Indiana (East)", Selected = false },
new SelectListItem() { Value="Central America Standard Time", Text="(GMT-06:00) Central America", Selected = false },
new SelectListItem() { Value="Central Standard Time", Text="(GMT-06:00) Central Time (US & Canada)", Selected = false },
new SelectListItem() { Value="Central Standard Time (Mexico)", Text="(GMT-06:00) Guadalajara, Mexico City, Monterrey", Selected = false },
new SelectListItem() { Value="Canada Central Standard Time", Text="(GMT-06:00) Saskatchewan", Selected = false },
new SelectListItem() { Value="US Mountain Standard Time", Text="(GMT-07:00) Arizona", Selected = false },
new SelectListItem() { Value="Mountain Standard Time (Mexico)", Text="(GMT-07:00) Chihuahua, La Paz, Mazatlan", Selected = false },
new SelectListItem() { Value="Mountain Standard Time", Text="(GMT-07:00) Mountain Time (US & Canada)", Selected = false },
new SelectListItem() { Value="Pacific Standard Time", Text="(GMT-08:00) Pacific Time (US & Canada)", Selected = false },
new SelectListItem() { Value="Pacific Standard Time (Mexico)", Text="(GMT-08:00) Tijuana, Baja California", Selected = false },
new SelectListItem() { Value="Alaskan Standard Time", Text="(GMT-09:00) Alaska", Selected = false },
new SelectListItem() { Value="Hawaiian Standard Time", Text="(GMT-10:00) Hawaii", Selected = false },
new SelectListItem() { Value="Samoa Standard Time", Text="(GMT-11:00) Midway Island, Samoa", Selected = false },
new SelectListItem() { Value="Dateline Standard Time", Text="(GMT-12:00) International Date Line West", Selected = false }
};
            return timezones;

        }

        [HttpPost]
        public JsonResult MailTemplateList(string name = null, string subj = null, string desc = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                EmailTemplateRepository dal = new EmailTemplateRepository();
                List<EmailTemplateModel> model = new List<EmailTemplateModel>();
                model = dal.GetAll().Where(x => x.Type == 0).ToList();
                if (!String.IsNullOrEmpty(name))
                {
                    model = model.Where(x => x.Id == Convert.ToInt32(name)).ToList();
                }
                if (!String.IsNullOrEmpty(subj))
                {
                    model = model.Where(x => x.Subject.ToLower().Contains(subj.ToLower())).ToList();
                }
                if (!String.IsNullOrEmpty(desc))
                {
                    model = model.Where(x => x.Description.ToLower().Contains(desc.ToLower())).ToList();
                }

                int count = model.Count;
                var Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetContactList(int id = 0, int jtStartIndex = 0, int jtPageSize = 0)
        {
            MassEmailingRepository dal = new MassEmailingRepository();
            try
            {
                var data = dal.GetContactById(id).ToList();
                int count = data.Count;
                List<ContactModel> Model1 = data.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult PrievewMail(int tempid)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            EmailTemplateModel bl = dal.GetById(tempid);
            return PartialView(bl);
        }

       
        [HttpPost]
        public JsonResult Delete(MassEmailingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                MassEmailingRepository dal = new MassEmailingRepository();
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
