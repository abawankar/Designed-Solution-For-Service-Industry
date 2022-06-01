using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationServices.Web;
using Domain.Interface;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class NewAccountController : Controller
    {
        static List<NewAccountModel> accountlist = new List<NewAccountModel>();
        static List<NBOModel> exportList = new List<NBOModel>();
        static List<NewAccountModel> modelList = new List<NewAccountModel>();

        
        public ActionResult Index()
        {
            CountryRepository ct = new CountryRepository();
            ViewBag.country = new SelectList(ct.GetAll(), "Id", "Name");

            NewAccountRepository act = new NewAccountRepository();
            ViewBag.account = new SelectList(act.GetAccountName(), "Id", "AccountName");

            ViewBag.type = new SelectList(new[] { 
                    new SelectListItem{Text="Client", Value="Client"},
                    new SelectListItem{Text="Supplier", Value="Supplier"},
                }, "Text", "Value");

            return View();
        }

        [HttpPost]
        public JsonResult List(int accountid = 0, string col = null, string type = null, int countryid = 0, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                List<NewAccountModel> model = new List<NewAccountModel>();
                if (accountid != 0)
                {
                    model = dal.GetIdList(accountid).ToList();
                }
                else
                {

                    if (col == "load")
                    {
                        model = dal.GetTop().ToList();
                    }
                    //else { model = dal.GetAll().ToList(); }


                    if (countryid != 0)
                    {
                        model = dal.GetAccountByCountryid(countryid).ToList();
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        //model = model.Where(x => (x.Type + " " + x.AccountName + " " + x.Industry.Name
                        //    + x.Website + " " + x.Description + " " + x.Phone + " " + x.Fax + " "
                        //    + x.Email + " " + x.Street + " " + x.City + " " + x.State + " "
                        //    + x.Country.Name).ToLower().Contains(name.ToLower())).ToList();
                        model = dal.GetBySearch(name).ToList();
                    }
                    if (!string.IsNullOrEmpty(type))
                    {
                        model = dal.GetAccountByType(type).ToList();
                    }
                }

                int count = model.Count;
                accountlist.Clear();
                accountlist = model;
                model = model.OrderBy(x => x.AccountName).ToList();
                List<NewAccountModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.InnerException.Message });
            }
        }

        public ActionResult FullDetails(int id)
        {
            NewAccountRepository dal = new NewAccountRepository();
            NewAccountModel bl = dal.GetById(id);
            return PartialView(bl);
        }

       
        [HttpPost]
        public JsonResult Create(NewAccountModel model)
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
                NewAccountRepository dal = new NewAccountRepository();
                var data = dal.GetAllByName(model.AccountName).Where(x => x.Country.Id == model.CountryId).ToList();
                if (data.Count == 0)
                {
                    dal.Insert(model);
                }
                else
                {
                    return Json(new { Result = "Error", Message = "Account already exist!" });
                }

                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HandleError]
       
        [HttpPost]
        public JsonResult Update(NewAccountModel model)
        {
            EmployeeRepository e = new EmployeeRepository();
            model.Empid = e.GetByName(User.Identity.Name.ToUpper());
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                NewAccountRepository dal = new NewAccountRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetAccountOptions(string type)
        {
            try
            {
                if (type == "Client")
                {
                    ClientRepository dal = new ClientRepository();
                    var list = dal.GetAll()
                        .Select(c => new { DisplayText = c.Name, Value = c.Name });
                    return Json(new { Result = "OK", Options = list });
                }
                if (type == "Supplier")
                {
                    SupplierRepository dal = new SupplierRepository();
                    var list = dal.GetAll()
                        .Select(c => new { DisplayText = c.Name, Value = c.Name });
                    return Json(new { Result = "OK", Options = list });
                }
                return Json(new { Result = "OK" });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult ViewMap(int id)
        {
            NewAccountRepository dal = new NewAccountRepository();
            INewAccount bl = dal.GetById(id);

            ViewBag.url = bl.Street + "," + bl.City + "," + bl.State + "," + bl.Country.Name;
            return PartialView();
        }

        #region ------ Account Contact -------

       
        [HttpPost]
        public JsonResult AddContact(ContactModel model, int id)
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
                dal.AddContact(model, id);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult EditContact(ContactModel model)
        {
            EmployeeRepository e = new EmployeeRepository();
            model.ModifierId = e.GetByName(User.Identity.Name.ToUpper());

            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                ContactRepository dal = new ContactRepository();
                dal.Edit(model);
                ContactModel model1 = dal.GetById(model.Id);
                return Json(new { Result = "OK", Record = model1 });
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
                NewAccountRepository dal = new NewAccountRepository();
                var model = dal.ContactList(id).OrderBy(x => x.FirstName);
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult DeleteContact(ContactModel model)
        {
            try
            {
                ContactRepository dal = new ContactRepository();
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
            ContactRepository dal = new ContactRepository();
            IContact bl = dal.GetById(id);

            ViewBag.url = bl.Street + "," + bl.City + "," + bl.StateProvince + "," + bl.Country.Name;
            return PartialView();
        }

        public ActionResult AccountSummary(int accountid)
        {
            ViewBag.accountid = accountid;
            return PartialView();
        }

        #endregion

        public ActionResult AccountData(int accountid)
        {
            try
            {
                NBORepository dal = new NBORepository();
                var list = from m in dal.GetAll().Where(x => x.ClientName.Id == accountid).GroupBy(x => x.Status)
                           select new
                           {
                               Status = m.Key.Name,
                               Value = m.Sum(x => x.ContractValue)
                           };
                return Json(list, "customer", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public ActionResult ExportData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Type", typeof(string)));
            dt.Columns.Add(new DataColumn("AccountName", typeof(string)));
            dt.Columns.Add(new DataColumn("Website", typeof(string)));
            dt.Columns.Add(new DataColumn("Description", typeof(string)));
            dt.Columns.Add(new DataColumn("Phone", typeof(string)));
            dt.Columns.Add(new DataColumn("Street", typeof(string)));
            dt.Columns.Add(new DataColumn("City", typeof(string)));
            dt.Columns.Add(new DataColumn("State", typeof(string)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));
            dt.Columns.Add(new DataColumn("Industry", typeof(string)));
            dt.Columns.Add(new DataColumn("TotalContacts", typeof(string)));

            GridView gv = new GridView();

            foreach (var item in accountlist.OrderBy(x => x.AccountName))
            {
                DataRow dr = dt.NewRow();
                dr["Type"] = item.Type;
                dr["AccountName"] = item.AccountName;
                dr["Website"] = item.Website;
                dr["Description"] = item.Description;
                dr["Phone"] = item.Phone;
                dr["Street"] = item.Street;
                dr["City"] = item.City;
                dr["State"] = item.State;
                dr["Country"] = item.Country.Name;
                dr["Industry"] = item.Industry.Name;
                dr["TotalContacts"] = item.Contact.Count();
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = "Account.xls";
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
        public JsonResult FileList(int id = 0, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                List<NBOModel> model = new List<NBOModel>();
                model = dal.GetAccountFiles(id).ToList();
                if (name != null)
                {
                    model = model.Where(x => x.ContactName.ToLower().Contains(name.ToLower())).ToList();
                }
                exportList.Clear();
                exportList = model.ToList();
                model = model.OrderByDescending(x => x.CheckinDate).ToList();
                int count = model.Count;

                List<NBOModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        public ActionResult ExportFiles()
        {
            string[] month = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Branch", typeof(string)));
            dt.Columns.Add(new DataColumn("FileHandler", typeof(string)));
            dt.Columns.Add(new DataColumn("Nature", typeof(string)));
            dt.Columns.Add(new DataColumn("RequestDate", typeof(string)));
            dt.Columns.Add(new DataColumn("FileNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("Client", typeof(string)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));
            dt.Columns.Add(new DataColumn("EventName", typeof(string)));
            dt.Columns.Add(new DataColumn("Pax", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckIn", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckOut", typeof(string)));
            dt.Columns.Add(new DataColumn("Representation", typeof(string)));
            dt.Columns.Add(new DataColumn("ContractValue", typeof(double)));
            dt.Columns.Add(new DataColumn("ContractCost", typeof(double)));
            dt.Columns.Add(new DataColumn("Margin", typeof(double)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));

            GridView gv = new GridView();
            NBORepository dal = new NBORepository();
            var data = exportList.OrderBy(x => x.RequestDate).ToList();

            foreach (var item1 in data.GroupBy(x => x.EventMonth).ToList())
            {
                DataRow drmonth = dt.NewRow();
                drmonth["Branch"] = month[Convert.ToInt32(item1.Key.Substring(0, 2))] + "-" + item1.Key.Substring(4, 2);
                dt.Rows.Add(drmonth);

                foreach (var item in item1)
                {
                    DataRow dr = dt.NewRow();
                    dr["Branch"] = item.Branch.Name;
                    dr["FileHandler"] = item.FileHandler.EmpName;
                    dr["Nature"] = item.Nature.Name;
                    dr["RequestDate"] = item.RequestDate.ToString("dd-MM-yyyy");
                    dr["FileNumber"] = item.FileNumber;
                    dr["Client"] = item.ClientName.Name;
                    dr["Country"] = item.ClientCountry.Name;
                    dr["EventName"] = item.EventName;
                    dr["Pax"] = item.PaxNo;
                    dr["CheckIn"] = Convert.ToDateTime(item.CheckinDate).ToString("dd-MM-yyyy");
                    dr["CheckOut"] = Convert.ToDateTime(item.CheckOutDate).ToString("dd-MM-yyyy");
                    dr["Representation"] = item.EnquirySource.Name;
                    dr["ContractValue"] = item.ContractValue;
                    dr["ContractCost"] = item.ContractCost;
                    dr["Margin"] = Convert.ToDouble(item.ContractValue) - Convert.ToDouble(item.ContractCost);
                    dr["Status"] = item.Status.Name;
                    dr["Remarks"] = item.Remarks;
                    dt.Rows.Add(dr);
                }
                DataRow MonthTotal = dt.NewRow();
                MonthTotal["Branch"] = "Month Total";
                MonthTotal["FileNumber"] = item1.Count();
                MonthTotal["Pax"] = item1.Sum(x => x.PaxNo);
                MonthTotal["ContractValue"] = Convert.ToDouble(item1.Sum(x => x.ContractValue)).ToString("##,###.00");
                MonthTotal["ContractCost"] = Convert.ToDouble(item1.Sum(x => x.ContractCost)).ToString("##,###.00");
                MonthTotal["Margin"] = Convert.ToDouble(item1.Sum(x => x.Margin)).ToString("##,###.00");
                dt.Rows.Add(MonthTotal);
            }
            DataRow GrandTotal = dt.NewRow();
            GrandTotal["Branch"] = "Grand Total";
            GrandTotal["FileNumber"] = data.Count();
            GrandTotal["Pax"] = data.Sum(x => x.PaxNo);
            GrandTotal["ContractValue"] = Convert.ToDouble(data.Sum(x => x.ContractValue)).ToString("##,###.00");
            GrandTotal["ContractCost"] = Convert.ToDouble(data.Sum(x => x.ContractCost)).ToString("##,###.00");
            GrandTotal["Margin"] = Convert.ToDouble(data.Sum(x => x.Margin)).ToString("##,###.00");
            dt.Rows.Add(GrandTotal);

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = "NBOData.xls";
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

        #region --- Add Account Type ----

       
        public ActionResult AddAccountType(string id, string list)
        {
            int actId = Convert.ToInt32(id);
            NewAccountRepository.AddActType(actId, list);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ListOfActType(int id = 0)
        {
            NewAccountRepository dal = new NewAccountRepository();
            try
            {
                var data = dal.GetById(id).ActType.OrderBy(x => x.Name);

                return Json(new { Result = "OK", Records = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult AccountList(int id = 0)
        {
            ViewBag.actid = id;
            return PartialView();
        }

        [HttpPost]
        public JsonResult SelectAccounType(int id = 0)
        {
            NewAccountRepository dal = new NewAccountRepository();
            AccountTypeRepository ct = new AccountTypeRepository();
            try
            {
                var act = dal.GetById(id).ActType.ToList();
                var ctid = from c in act
                           select c.Id;

                List<AccountTypeModel> list = new List<AccountTypeModel>();
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
        public JsonResult DeleteAccountType(AccountTypeModel model, int actid)
        {
            try
            {
                NewAccountRepository dal = new NewAccountRepository();
                dal.DeleteActType(model.Id, actid);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        #endregion

        [HttpPost]
        public JsonResult GetContactByClientid(int clientid = 0)
        {
            NewAccountRepository dal = new NewAccountRepository();
            try
            {
                if (clientid != 0)
                {
                    var data = dal.GetClientIdList(clientid).OrderBy(x => x.FirstName)
                           .Select(c => new { DisplayText = c.FirstName + " " + c.LastName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.OrderBy(x => x.DisplayText) });
                }
                else
                {
                    var data = dal.GetClientIdList(0).DefaultIfEmpty()
                           .Select(c => new { DisplayText = c.FirstName + " " + c.LastName, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.OrderBy(x => x.DisplayText) });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public ActionResult Import()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ImportAccount(HttpPostedFileWrapper imageFile)
        {
            var fileName = "ImportAccount.csv";
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Upload/")), fileName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            imageFile.SaveAs(imagePath);
            List<NewAccountModel> model = new List<NewAccountModel>();
            string filename = imagePath;
            using (var reader = new StreamReader(System.IO.File.OpenRead(filename)))
            {
                int ii = 1;
                while (!reader.EndOfStream)
                {
                    NewAccountRepository dal = new NewAccountRepository();


                    IndustryRepository ind = new IndustryRepository();
                    CountryRepository cont = new CountryRepository();
                    EmployeeRepository emp = new EmployeeRepository();
                    AccountTypeRepository act = new AccountTypeRepository();

                    NewAccountModel bl = new NewAccountModel();
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var data = dal.GetAllByName(values[0].ToString()).Where(x => x.Country.Id == Convert.ToInt32(values[13])).ToList();
                    if (data.Count > 0)
                    {
                        bl.dup = 1;
                    }
                    bl.Id = ii;
                    bl.AccountName = values[0].ToString();
                    bl.AccountId = Convert.ToInt32(values[1]);
                    bl.Type = values[2].ToString();
                    bl.Website = values[3].ToString();
                    bl.Description = values[4].ToString();
                    bl.Phone = values[5].ToString();
                    bl.Street = values[6].ToString().Replace(';', ',');
                    bl.City = values[7].ToString();
                    bl.State = values[8].ToString();
                    bl.Fax = values[9].ToString();
                    bl.Email = values[10].ToString();
                    bl.Date = Convert.ToDateTime(values[11]);
                    bl.ZipPostalCode = values[16].ToString();
                    try
                    {
                        IIndustry i = ind.GetById(Convert.ToInt32(values[12]));
                        ICountry c = cont.GetById(Convert.ToInt32(values[13]));
                        IEmployee e = emp.GetById(Convert.ToInt32(values[14]));
                        bl.Industry = i;
                        bl.Country = c;
                        bl.Owner = e;

                    }
                    catch (Exception) { }

                    if (!String.IsNullOrEmpty(values[15]))
                    {
                        var actType = values[15].Split('|');
                        List<IAccountType> atList = new List<IAccountType>();
                        foreach (var a in actType)
                        {
                            IAccountType at = act.GetById(Convert.ToInt32(a));
                            atList.Add(at);
                        }
                        bl.ActType = atList;
                    }

                    model.Add(bl);
                    ii++;
                }
            }
            modelList = model.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult ImportAccountList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int count = modelList.Count;
                List<NewAccountModel> Model1 = modelList.Skip(jtStartIndex).Take(jtPageSize).ToList();
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
            dal.InsertBulk(modelList);
            return RedirectToAction("Index");
        }

       
        public ActionResult CreateMail(int id, string type)
        {
            ViewBag.type = type;
            NewAccountRepository dal = new NewAccountRepository();
            INewAccount bl = dal.GetById(id);
            return View(bl);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SendMail(AccountMailModel obj)
        {
            try
            {
                MailMessage msg = new MailMessage();
                EmployeeRepository e = new EmployeeRepository();
                IEmployee emp = e.GetBy(User.Identity.Name.ToUpper());
                //msg.From = new MailAddress("arvind@aprilsourcing.com", emp.EmpName);
                msg.From = new MailAddress("AgneloF1@1001events.com", emp.EmpName);
                obj.OwnerId = emp.Id;
                string[] tomail = obj.To.Split(';');
                foreach (var item in tomail)
                {
                    if (item != "")
                    {
                        msg.To.Add(new MailAddress(item));
                    }
                }
                if (!string.IsNullOrEmpty(obj.CC))
                {
                    msg.CC.Add(new MailAddress(obj.CC));
                }
                msg.Subject = obj.Subject;
                msg.Body = obj.MailBody;
                //msg.Headers.Add("Disposition-Notification-To", emp.MailId);
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                msg.ReplyToList.Add(new MailAddress(emp.MailId, emp.EmpName));
                msg.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(obj.Replyto))
                {
                    string[] disp = obj.Replyto.Split('@');
                    msg.ReplyToList.Add(new MailAddress(obj.Replyto, disp[0]));
                }
                EmailSetting.SendEmail(msg);
                NewAccountRepository.AddMailLog(obj);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (obj.type == "L")
            {
                return RedirectToAction("Index", "LeastProducer");
            }
            return RedirectToAction("Index", "NewAccount");
        }

        
    }

}
