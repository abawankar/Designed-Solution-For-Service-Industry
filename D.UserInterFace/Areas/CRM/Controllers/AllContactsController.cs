using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationServices.Web;
using Domain.Interface.Master;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class AllContactsController : Controller
    {
        static List<ContactModel> contactlist = new List<ContactModel>();
        static List<ContactModel> modelList = new List<ContactModel>();
        //
        // GET: /AllContacts/

       
        public ActionResult Index()
        {
            ViewBag.type = new SelectList(new[] { 
                    new SelectListItem{Text="Client", Value="Client"},
                    new SelectListItem{Text="Supplier", Value="Supplier"},
                }, "Text", "Value");

            return View();
        }

        public ActionResult GetCountry()
        {
            try
            {
                CountryRepository dal = new CountryRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetCity()
        {
            try
            {
                ContactRepository cont = new ContactRepository();
                var list = from m in cont.GetCity().OrderBy(x => x.City)
                           select new { Id = m.City.ToUpperIgnoreNull(), Name = m.City.ToUpperIgnoreNull() };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetAccount()
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                var list = from m in dal.GetAccountName().OrderBy(x => x.AccountName)
                           select new { Id = m.Id, Name = m.AccountName };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetContactType()
        {
            try
            {
                ContactTypeRepository dal = new ContactTypeRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetAccountType()
        {
            try
            {
                AccountTypeRepository dal = new AccountTypeRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetAccountCat()
        {
            try
            {
                IndustryRepository dal = new IndustryRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetContactGroup()
        {
            try
            {
                ContactGroupRepository dal = new ContactGroupRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetAccountAddress(int actid)
        {
            NewAccountRepository dal = new NewAccountRepository();
            var model = dal.GetIdList(actid).ToList();
            return Json(new { Result = model });
        }

        [HttpPost]
        public JsonResult GetContactInfo(int contactid)
        {
            ContactRepository dal = new ContactRepository();
            var model = dal.GetIdList(contactid).ToList();
            return Json(new { Result = model });
        }

        [HttpPost]
        public JsonResult ContactList(string col = null, string find = null, string actCat = null, string contType = null, string type = null, string contgp = null, string city = null, string ctryid = null, string acname = null, string first = null, string last = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            NewAccountRepository ac = new NewAccountRepository();
            ContactGroupRepository dal = new ContactGroupRepository();
            ContactRepository ct = new ContactRepository();
            try
            {
                List<ContactModel> list = new List<ContactModel>();

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
                    if (!string.IsNullOrEmpty(type))
                    {
                        list = ac.ContactListByType(type).ToList();
                    }
                    else
                    {
                        if (col == "load")
                        {
                            list = ct.GetTop().ToList();
                        }
                        else
                        {
                            //list = ct.GetAll().ToList();
                        }

                    }
                }
                // Get all contact by country id
                if (!string.IsNullOrEmpty(ctryid))
                {
                    string[] idlist = ctryid.Split(',');
                    if (list.Count != 0)
                    {
                        list = list.Where(x => x.Country.Id.ToString() != null && idlist.Contains(x.Country.Id.ToString())).ToList();
                    }
                    else
                    {
                        foreach (var ids in idlist)
                        {
                            list = list.Concat(ct.GetByCountryid(Convert.ToInt32(ids))).ToList();
                        }
                    }

                }
                // get all by account category
                if (!string.IsNullOrEmpty(actCat))
                {
                    string[] idlist = actCat.Split(',');
                    List<NewAccountModel> actdata = new List<NewAccountModel>();
                    foreach (var ids in idlist)
                    {
                        actdata = actdata.Concat(ac.GetByIndustryid(Convert.ToInt32(ids))).ToList();
                    }
                    list = ac.GetContactModel(actdata).ToList();
                }

                if (!string.IsNullOrEmpty(acname))
                {
                    string[] idlist = acname.Split(',');
                    List<NewAccountModel> actdata = new List<NewAccountModel>();
                    foreach (var ids in idlist)
                    {
                        actdata = actdata.Concat(ac.GetIdList(Convert.ToInt32(ids))).ToList();
                    }
                    list = ac.GetContactModel(actdata).ToList();
                }
                if (!string.IsNullOrEmpty(city))
                {
                    string[] idlist = city.Split(',');
                    if (city == "0")
                    {

                    }
                    else
                    {
                        if (list.Count != 0)
                        {
                            list = list.Where(x => idlist.Contains(x.City.ToUpper())).ToList();
                        }
                        else
                        {
                            foreach (var ids in idlist)
                            {
                                list = list.Concat(ct.GetByCity(ids).ToList()).ToList();
                            }
                        }

                    }

                }
                if (!string.IsNullOrEmpty(first))
                {
                    if (list.Count != 0)
                    {
                        list = list.Where(x => x.FirstName.ToUpper().StartsWith(first.ToUpper())).ToList();
                    }
                    else
                    {
                        list = ct.GetByFirstName(first).ToList();
                    }

                }
                if (!string.IsNullOrEmpty(last))
                {
                    if (list.Count != 0)
                    {
                        list = list.Where(x => x.LastName.ToUpper().StartsWith(last.ToUpper())).ToList();
                    }
                    else
                    {
                        list = ct.GetByLastName(last).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(contType))
                {
                    string[] idlist = contType.Split(',');
                    if (list.Count != 0)
                    {
                        list = list.Where(x => x.ContType.Any(y => idlist.Contains(y.Id.ToString()))).ToList();
                    }
                    else
                    {
                        foreach (var ids in idlist)
                        {
                            list = list.Concat(ct.GetByContactType(Convert.ToInt32(ids))).ToList();
                        }
                    }

                }
                if (!String.IsNullOrEmpty(find))
                {
                    if (list.Count != 0)
                    {
                        list = list.Where(x => (x.AccountName + " " + x.FirstName
                        + x.MiddleName + " " + x.LastName + " " + x.Title + " " + x.Department + " "
                        + x.Phone + " " + x.PhoneDirect + " " + x.Mobile + " " + x.Messanger + " "
                        + x.Email + " " + x.Street + " " + x.StateProvince + " "
                        + x.City).ToLower().Contains(find.ToLower())).ToList();
                    }
                    else
                    {
                        list = ct.GetBySearch(find).ToList();
                    }

                }
                int count = list.Count;
                contactlist = list.ToList();
                list = list.OrderBy(x => x.FirstName).ToList();
                List<ContactModel> Model1 = list.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public ActionResult ExportData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Title", typeof(string)));
            dt.Columns.Add(new DataColumn("FirstName", typeof(string)));
            dt.Columns.Add(new DataColumn("MiddleName", typeof(string)));
            dt.Columns.Add(new DataColumn("LastName", typeof(string)));
            dt.Columns.Add(new DataColumn("Email", typeof(string)));
            dt.Columns.Add(new DataColumn("Phone", typeof(string)));
            dt.Columns.Add(new DataColumn("Mobile", typeof(string)));
            dt.Columns.Add(new DataColumn("Fax", typeof(string)));
            dt.Columns.Add(new DataColumn("Department", typeof(string)));
            dt.Columns.Add(new DataColumn("Street", typeof(string)));
            dt.Columns.Add(new DataColumn("City", typeof(string)));
            dt.Columns.Add(new DataColumn("StateProvince", typeof(string)));
            dt.Columns.Add(new DataColumn("ZipPostalCode", typeof(string)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));
            dt.Columns.Add(new DataColumn("AccountName", typeof(string)));

            GridView gv = new GridView();

            foreach (var item in contactlist.OrderBy(x => x.FirstName))
            {
                DataRow dr = dt.NewRow();
                dr["Title"] = item.Title;
                dr["FirstName"] = item.FirstName;
                dr["MiddleName"] = item.MiddleName;
                dr["LastName"] = item.LastName;
                dr["Email"] = item.Email;
                dr["Phone"] = item.Phone;
                dr["Mobile"] = item.Mobile;
                dr["Fax"] = item.Fax;
                dr["Department"] = item.Department;
                dr["Country"] = item.Country.Name;
                dr["Street"] = item.Street;
                dr["City"] = item.City;
                dr["StateProvince"] = item.StateProvince;
                dr["ZipPostalCode"] = item.ZipPostalCode;
                dr["AccountName"] = item.AccountName;
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = "AccountContact.xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);

            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetAccountOptions()
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                var list = dal.GetAll()
                                .Select(c => new { DisplayText = c.AccountName, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult AddContact(ContactModel model)
        {
            model.CreationDate = Convert.ToDateTime(MyExtension.UAETime().ToShortDateString());
            EmployeeRepository e = new EmployeeRepository();
            model.CreatorId = e.GetByName(User.Identity.Name.ToUpper());
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                NewAccountRepository dal = new NewAccountRepository();
                dal.AddContact(model, model.Accountid);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        public ActionResult Import()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ImportContacts(HttpPostedFileWrapper imageFile)
        {
            ContactTypeRepository act = new ContactTypeRepository();
            var fileName = "ImportContacts.csv";
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Upload/")), fileName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            imageFile.SaveAs(imagePath);
            List<ContactModel> model = new List<ContactModel>();
            string filename = imagePath;
            using (var reader = new StreamReader(System.IO.File.OpenRead(filename)))
            {
                int ii = 1;
                while (!reader.EndOfStream)
                {
                    EmployeeRepository emp = new EmployeeRepository();
                    CountryRepository ct = new CountryRepository();
                    ContactModel bl = new ContactModel();

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
                    bl.EmailOpt = Convert.ToBoolean(values[11]);
                    bl.Department = values[12].ToString();
                    bl.Street = values[13].ToString().Replace(';', ',');
                    bl.City = values[14].ToString();
                    bl.StateProvince = values[15].ToString();
                    bl.ZipPostalCode = values[16].ToString();
                    ICountry c = ct.GetById(Convert.ToInt32(values[17]));
                    bl.Accountid = Convert.ToInt32(values[18]);
                    bl.AccountName = values[19].ToString();
                    bl.Country = c;
                    IEmployee e = emp.GetById(Convert.ToInt32(values[21]));
                    bl.Creater = e;
                    bl.CreationDate = Convert.ToDateTime(values[22]);
                    bl.CreatorId = Convert.ToInt32(values[21]);
                    if (!String.IsNullOrEmpty(values[20]))
                    {
                        var actType = values[20].Split('|');
                        List<IContactType> atList = new List<IContactType>();
                        foreach (var a in actType)
                        {
                            IContactType at = act.GetById(Convert.ToInt32(a));
                            atList.Add(at);
                        }
                        bl.ContType = atList;
                    }

                    model.Add(bl);
                    ii++;
                }
            }

            modelList = model.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult ImportContactList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int count = modelList.Count;
                List<ContactModel> Model1 = modelList.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.InnerException.Message });
            }
        }

        public ActionResult ProcessImport()
        {
            NewAccountRepository dal = new NewAccountRepository();
            foreach (var item in modelList)
            {
                item.CountryId = item.Country.Id;
                dal.AddContact(item, item.Accountid);
            }

            return RedirectToAction("Index");
        }

        #region --- Add Contact Type ----

        [HttpPost]
        public JsonResult ListOfContType(int id = 0)
        {
            ContactRepository dal = new ContactRepository();
            try
            {
                var data = dal.GetById(id).ContType.OrderBy(x => x.Name);

                return Json(new { Result = "OK", Records = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public ActionResult AddContactType(string id, string list)
        {
            int actId = Convert.ToInt32(id);
            ContactRepository.AddContType(actId, list);
            return RedirectToAction("Index");
        }

        public ActionResult ContTypeList(int id = 0)
        {
            ViewBag.actid = id;
            return PartialView();
        }

        [HttpPost]
        public JsonResult SelectContactType(int id = 0)
        {
            ContactRepository dal = new ContactRepository();
            ContactTypeRepository ct = new ContactTypeRepository();
            try
            {
                var act = dal.GetById(id).ContType.ToList();
                var ctid = from c in act
                           select c.Id;

                List<ContactTypeModel> list = new List<ContactTypeModel>();
                list = ct.GetAll().ToList();

                list = list.Where(c => !ctid.Contains(c.Id)).ToList();
                list = list.OrderBy(x => x.Name).ToList();
                return Json(new { Result = "OK", Records = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult DeleteContactType(ContactTypeModel model, int contid)
        {
            try
            {
                ContactRepository dal = new ContactRepository();
                dal.DeleteContType(model.Id, contid);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        #endregion

       
        public ActionResult Duplicate()
        {
            ContactRepository dal = new ContactRepository();
            var duplicate = dal.GetAll().GroupBy(x => x.Email)
                .Where(g => g.Count() > 1)
                .Select(g => new { mail = g.Key });

            ViewBag.duplicate = new SelectList(duplicate, "mail", "mail"); ;
            return View();
        }

        [HttpPost]
        public JsonResult boundList(string mailid = null, string type = null)
        {
            ContactRepository dal = new ContactRepository();
            ContactGroupRepository cg = new ContactGroupRepository();
            MassEmailingRepository mass = new MassEmailingRepository();
            OneStayInTouchRepository one = new OneStayInTouchRepository();

            List<ContactModel> model = dal.GetAll().Where(x => x.Email == mailid).ToList();
            List<ContactModel> unboundlist = new List<ContactModel>();
            List<ContactModel> boundlist = new List<ContactModel>();

            var idlist = from m in model
                         select m.Id;

            var cglist = cg.GetAll().SelectMany(x => x.ContactList).Where(y => idlist.Contains(y.Id)).ToList();
            var masslist = mass.GetAll().SelectMany(x => x.ContactGroup).Where(y => idlist.Contains(y.Id)).ToList();
            var onelist = one.GetAll().Select(x => x.Contact).Where(y => idlist.Contains(y.Id)).ToList();
            var combine = cglist.Concat(masslist).Concat(onelist).ToList();

            var unbid = from m in combine
                        select m.Id;

            unboundlist = model.Where(x => !unbid.Contains(x.Id)).ToList();

            boundlist = model.Where(x => unbid.Contains(x.Id)).ToList();

            if (type == "u")
            {
                model = unboundlist.ToList();
            }
            if (type == "b")
            {
                model = boundlist.ToList();
            }

            try
            {
                return Json(new { Result = "OK", Records = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.InnerException.Message });
            }
        }

        public ActionResult MergeContact(string bid = null, string uid = null)
        {
            ContactRepository dal = new ContactRepository();
            dal.MergeContact(bid, uid);
            return RedirectToAction("Duplicate");
        }

    }
}
