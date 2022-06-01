using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class OneStayInTouchController : Controller
    {
        //
        // GET: /OneStayInTouch/

       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                OneStayInTouchRepository dal = new OneStayInTouchRepository();
                List<OneStayInTouchModel> model = new List<OneStayInTouchModel>();
                model = dal.GetAll().OrderByDescending(x => x.Date).ToList();
                int count = model.Count;
                List<OneStayInTouchModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult ListOfContact(string contType = null, string actType = null, string actCat = null, string type = null, string contgp = null, string city = null, string ctryid = null, string acname = null, string first = null, string last = null, int jtStartIndex = 0, int jtPageSize = 0)
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
                    act = ac.GetAll().ToList();
                    if (!string.IsNullOrEmpty(type))
                    {
                        act = act.Where(x => x.Type == type).ToList();
                    }
                    if (!string.IsNullOrEmpty(actCat))
                    {
                        string[] idlist = actCat.Split(',');
                        act = act.Where(x => idlist.Contains(x.Industry.Id.ToString())).ToList();
                    }
                    if (!string.IsNullOrEmpty(actType))
                    {
                        string[] idlist = actType.Split(',');
                        act = act.Where(x => x.ActType.Any(y => idlist.Contains(y.Id.ToString()))).ToList();
                    }
                    if (!string.IsNullOrEmpty(acname))
                    {
                        string[] idlist = acname.Split(',');
                        act = act.Where(x => idlist.Contains(x.Id.ToString())).ToList();
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

       
        public ActionResult ContactList()
        {

            ViewBag.type = new SelectList(new[] { 
                    new SelectListItem{Text="Client", Value="Client"},
                    new SelectListItem{Text="Supplier", Value="Supplier"},
                }, "Text", "Value");


            return View();
        }

        [HttpPost]
        public JsonResult GetCityByCountry(int contid)
        {
            ContactRepository cont = new ContactRepository();
            var data = from m in cont.GetAll()
                       select new { City = m.City.ToUpper() };

            if (contid != 0)
            {
                var cdata = from m in cont.GetAll().Where(x => x.Country.Id == contid)
                            select new { City = m.City.ToUpper() };

                return Json(new { Result = cdata.Distinct() });
            }
            else
            {
                return Json(new { Result = data.Distinct() });
            }

        }

       
        public ActionResult MassStayInTouch(string idlist)
        {
            ViewBag.contactlist = idlist;
            ContactRepository dal = new ContactRepository();
            List<ContactModel> contactlist = new List<ContactModel>();
            string[] list = idlist.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int id = Convert.ToInt32(list[i]);
                ContactModel contact = dal.GetById(id);
                contactlist.Add(contact);
            }
            EmailTemplateRepository e = new EmailTemplateRepository();
            ViewBag.noteTemplate = new SelectList(e.GetAll().Where(x => x.Type == 1), "Id", "TemplateName");

            return View(contactlist);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SendMassStayInTouch(OneStayInTouchModel model)
        {
            OneStayInTouchRepository dal = new OneStayInTouchRepository();
            model.Date = MyExtension.UAETime();
            model.Emp = User.Identity.Name.ToUpper();
            string[] list = model.contactlist.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int to = Convert.ToInt32(list[i]);
                model.to = to;
                dal.InsertBulk(model);
            }

            return RedirectToAction("Index", "OneStayInTouch");

        }

       
        [HttpPost]
        public JsonResult Delete(OneStayInTouchModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                OneStayInTouchRepository dal = new OneStayInTouchRepository();
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
