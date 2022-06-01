using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Web;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class PotentialAccountController : Controller
    {
        static List<PotentialAccountModel> modelList = new List<PotentialAccountModel>();
        static List<PotentialContactModel> contactList = new List<PotentialContactModel>();

        //
        // GET: /PotentialAccount/

       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                PotentialAccountRepository dal = new PotentialAccountRepository();
                List<PotentialAccountModel> model = new List<PotentialAccountModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }

                int count = model.Count;
                model = model.OrderBy(x => x.AccountName).ToList();
                List<PotentialAccountModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Create(PotentialAccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                PotentialAccountRepository dal = new PotentialAccountRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Update(PotentialAccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                PotentialAccountRepository dal = new PotentialAccountRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult Delete(PotentialAccountModel model)
        {
            try
            {
                PotentialAccountRepository dal = new PotentialAccountRepository();
                dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public ActionResult ViewMap(int id)
        {
            PotentialAccountRepository dal = new PotentialAccountRepository();
            IPotentialAccount bl = dal.GetById(id);

            ViewBag.url = bl.Street + "," + bl.City + "," + bl.State + "," + bl.Country.Name;
            return PartialView();
        }

        public ActionResult ConvertToAccount(int id)
        {
            ViewBag.id = id;
            ViewBag.type = new SelectList(new[]{
                new SelectListItem{Value="Client",Text="Client"},
                new SelectListItem{Value="Supplier",Text="Supplier"}
                }, "Value", "Text");

            IndustryRepository ind = new IndustryRepository();
            ViewBag.industry = new SelectList(ind.GetAll(), "Id", "Name");

            CountryRepository cont = new CountryRepository();
            ViewBag.country = new SelectList(cont.GetAll(), "Id", "Name");

            return PartialView();
        }

       
        public ActionResult Convert(int id)
        {
            PotentialAccountRepository dal = new PotentialAccountRepository();
            PotentialAccountModel model = dal.GetById(id);
            EmployeeRepository e = new EmployeeRepository();
            int empid = e.GetByName(User.Identity.Name.ToUpper());
            if (dal.Convert(model, empid) == true)
            {
                var data = dal.Delete(model.Id);
                return RedirectToAction("Index", "NewAccount");
            }
            else
            {
                return RedirectToAction("Index", "PotentialAccount");
            }

        }

        #region ------ Account Contact -------

       
        [HttpPost]
        public JsonResult AddContact(PotentialContactModel model, int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                PotentialAccountRepository dal = new PotentialAccountRepository();
                dal.AddContact(model, id);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult EditContact(PotentialContactModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                PotentialContactRepository dal = new PotentialContactRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListContact(int id)
        {
            try
            {
                PotentialAccountRepository dal = new PotentialAccountRepository();
                var model = dal.ContactList(id).OrderBy(x => x.FirstName);
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult DeleteContact(PotentialContactModel model)
        {
            try
            {
                PotentialContactRepository dal = new PotentialContactRepository();
                dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public ActionResult ViewContactMap(int id)
        {
            PotentialContactRepository dal = new PotentialContactRepository();
            IPotentialContact bl = dal.GetById(id);

            ViewBag.url = bl.Street + "," + bl.City + "," + bl.StateProvince + "," + bl.Country.Name;
            return PartialView();
        }

        #endregion

        #region --- Import Leads Contact  --

       
        public ActionResult ImportContact()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ImportLeadsContact(HttpPostedFileWrapper imageFile)
        {
            var fileName = "ImportLeadsContact.csv";
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Upload/")), fileName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            imageFile.SaveAs(imagePath);
            List<PotentialContactModel> model = new List<PotentialContactModel>();
            string filename = imagePath;
            var reader = new StreamReader(System.IO.File.OpenRead(filename));
            int ii = 1;
            while (!reader.EndOfStream)
            {
                IndustryRepository ind = new IndustryRepository();
                CountryRepository ct = new CountryRepository();
                PotentialAccountRepository dal = new PotentialAccountRepository();

                PotentialContactModel bl = new PotentialContactModel();
                var line = reader.ReadLine();
                var values = line.Split(',');
                bl.Id = ii;
                bl.Salutation = values[0].ToString();
                bl.FirstName = values[1].ToString();
                bl.MiddleName = values[2].ToString();
                bl.LastName = values[3].ToString();
                bl.Title = values[4].ToString();
                bl.Email = values[5].ToString();
                bl.Phone = values[6].ToString();
                bl.PhoneDirect = values[7].ToString();
                bl.Mobile = values[8].ToString();
                bl.Fax = values[9].ToString();
                bl.Messanger = values[10].ToString();
                bl.EmailOpt = System.Convert.ToBoolean(values[11]);
                bl.Department = values[12].ToString();
                bl.Street = values[13].ToString().Replace(';', ',');
                bl.City = values[14].ToString();
                bl.StateProvince = values[15].ToString();
                bl.ZipPostalCode = values[16].ToString();
                ICountry c = ct.GetById(System.Convert.ToInt32(values[17]));
                bl.Accountid = System.Convert.ToInt32(values[18]);
                bl.AccountName = values[19].ToString();
                bl.Country = c;
                model.Add(bl);
                ii++;
            }
            contactList = model.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult ImportContactList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int count = contactList.Count;
                List<PotentialContactModel> Model1 = contactList.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.InnerException.Message });
            }
        }

        public ActionResult ProcessContactImport()
        {
            PotentialAccountRepository dal = new PotentialAccountRepository();
            foreach (var item in contactList)
            {
                dal.AddContact(item, item.Accountid);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region -- Import Leads ---

       
        public ActionResult Import()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ImportLeads(HttpPostedFileWrapper imageFile)
        {
            var fileName = "ImportLeads.csv";
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Upload/")), fileName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            imageFile.SaveAs(imagePath);
            List<PotentialAccountModel> model = new List<PotentialAccountModel>();
            string filename = imagePath;
            using (var reader = new StreamReader(System.IO.File.OpenRead(filename)))
            {
                int ii = 1;
                while (!reader.EndOfStream)
                {
                    IndustryRepository ind = new IndustryRepository();
                    CountryRepository cont = new CountryRepository();
                    PotentialAccountRepository dal = new PotentialAccountRepository();

                    PotentialAccountModel bl = new PotentialAccountModel();
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    bl.Id = ii;
                    bl.AccountName = values[0].ToString();
                    bl.AccountId = 0;
                    bl.Type = values[1].ToString();
                    bl.Website = values[2].ToString();
                    bl.Description = values[3].ToString();
                    bl.Phone = values[4].ToString();
                    bl.Street = values[5].ToString().Replace(';', ',');
                    bl.City = values[6].ToString();
                    bl.State = values[7].ToString();
                    bl.ZipPostalCode = values[10].ToString();
                    IIndustry i = ind.GetById(System.Convert.ToInt32(values[8]));
                    ICountry c = cont.GetById(System.Convert.ToInt32(values[9]));
                    bl.Industry = i;
                    bl.Country = c;
                    model.Add(bl);
                    ii++;
                }
            }
            modelList = model.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult ImportLeadList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int count = modelList.Count;
                List<PotentialAccountModel> Model1 = modelList.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.InnerException.Message });
            }
        }

        public ActionResult ProcessImport()
        {
            PotentialAccountRepository dal = new PotentialAccountRepository();
            dal.InsertBulk(modelList);
            return RedirectToAction("Index");
        }

        #endregion

    }
}
