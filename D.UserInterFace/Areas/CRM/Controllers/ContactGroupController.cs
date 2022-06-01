using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class ContactGroupController : Controller
    {
        //
        // GET: /ContatGroup/

       
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                ContactGroupRepository dal = new ContactGroupRepository();
                List<ContactGroupModel> model = new List<ContactGroupModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().Where(x => x.Name.Contains(name)).ToList();
                }
                int count = model.Count;
                var data = from m in model
                           select new ContactGroupModel
                           {
                               Id = m.Id,
                               Date = m.Date,
                               Name = m.Name,
                               Note = m.Note,
                               Empid = m.Empid
                           };


                List<ContactGroupModel> Model1 = data.OrderBy(x => x.Name).Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Create(ContactGroupModel model)
        {
            model.Date = Convert.ToDateTime(MyExtension.UAETime().ToShortDateString());
            EmployeeRepository e = new EmployeeRepository();
            model.Empid = e.GetByName(User.Identity.Name.ToUpper());
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                ContactGroupRepository dal = new ContactGroupRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Update(ContactGroupModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                ContactGroupRepository dal = new ContactGroupRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Delete(ContactGroupModel model)
        {
            try
            {
                ContactGroupRepository dal = new ContactGroupRepository();
                var data = dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = data });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        public ActionResult AddContact(string id, string list)
        {
            int groupid = Convert.ToInt32(id);
            ContactGroupRepository.AddContact(groupid, list);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ListOfContact(string contType = null, string actType = null, string actCat = null, string type = null, int id = 0, string city = null, string ctryid = null, string acname = null, string first = null, string last = null, int jtStartIndex = 0, int jtPageSize = 0)
        {
            NewAccountRepository ac = new NewAccountRepository();
            ContactGroupRepository dal = new ContactGroupRepository();
            ContactRepository ct = new ContactRepository();
            try
            {
                var contact = dal.GetById(id).ContactList.ToList();
                var ctid = from c in contact
                           select c.Id;
                List<ContactModel> list = new List<ContactModel>();
                List<NewAccountModel> act = new List<NewAccountModel>();
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

                list = list.Where(c => !ctid.Contains(c.Id)).ToList();
                int count = list.Count;
                List<ContactModel> Model1 = list.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult ContactList(int id = 0)
        {
            ContactGroupRepository dal = new ContactGroupRepository();

            ViewBag.contactid = new SelectList(dal.GetAll().Where(x => x.Id == id), "Id", "Name");

            ViewBag.type = new SelectList(new[] { 
                    new SelectListItem{Text="Client", Value="Client"},
                    new SelectListItem{Text="Supplier", Value="Supplier"},
                }, "Text", "Value");

            return PartialView();
        }

        [HttpPost]
        public JsonResult GetContactList(int groupid = 0, int jtStartIndex = 0, int jtPageSize = 0)
        {
            ContactGroupRepository dal = new ContactGroupRepository();
            try
            {
                var data = dal.GetById(groupid).ContactList.ToList();
                int count = data.Count;
                data = data.OrderBy(x => x.FirstName).ToList();
                var Model1 = data.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
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

       
        [HttpPost]
        public JsonResult DeleteContact(ContactModel model, int groupid)
        {
            try
            {
                ContactGroupRepository dal = new ContactGroupRepository();
                dal.DeleteContact(model.Id, groupid);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }



    }
}
