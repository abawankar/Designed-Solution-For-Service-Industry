
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.CRM;
using DAL.Master;
using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using Domain.Interface;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Models;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class NewAccountModel : Domain.Implementation.CRM.NewAccount
    {
        public int IndustryId { get; set; }
        public int CountryId { get; set; }
        public int Empid { get; set; }
        public int dup { get; set; }
    }

    public class NewAccountRepository : Repository<NewAccountModel>
    {

        public override NewAccountModel GetById(int id)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            NewAccountModel model = AutoMapper.Mapper.Map<NewAccountModel>(dal.GetById(id));

            return model;
        }

        public override IList<NewAccountModel> GetAll()
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetAll());

            return model;
        }

        public IList<NewAccount> GetAccountName()
        {
            NewAccountDAL dal = new NewAccountDAL();
            List<NewAccount> list = dal.GetAccountName().OrderBy(x => x.AccountName).ToList();
            return list;
        }

        public IList<NewAccountModel> GetTop()
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetTop());

            return model;
        }

        public IList<NewAccountModel> GetIdList(int id)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetIdList(id));

            return model;
        }

        public IList<NewAccountModel> GetByCity(string city)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetByCity(city));

            return model;
        }

        public IList<NewAccountModel> GetAccountByCountryid(int id)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetAccountByCountryid(id));

            return model;
        }

        public IList<NewAccountModel> GetByIndustryid(int id)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetByIndustryId(id));

            return model;
        }

        public IList<NewAccountModel> GetAccountByType(string type)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetAccountByType(type));

            return model;
        }

        public IList<NewAccountModel> GetBySearch(string search)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetBySearch(search));

            return model;
        }

        public IList<NewAccountModel> GetAllByName(string name)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>();
            AutoMapper.Mapper.CreateMap<NewAccount, NewAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<NewAccountModel> model = AutoMapper.Mapper.Map<List<NewAccountModel>>(dal.GetByName(name));

            return model;
        }

        public List<ContactModel> GetByName(string name)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByName(name).SelectMany(x => x.Contact));

            return model;
        }

        public List<ContactModel> GetContactModel(List<NewAccountModel> actModel)
        {
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(actModel.SelectMany(x => x.Contact));

            return model;
        }

        public List<ContactModel> GetByActid(int actid)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByAcId(actid).Contact);

            return model;
        }

        public List<ContactModel> GetClientIdList(int clientid)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetClientIdList(clientid).SelectMany(x => x.Contact));
            model.Add(new ContactModel { Id = 0, FirstName = "* Select *" });

            return model;
        }

        public List<ContactModel> GetByType(string type)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByType(type).SelectMany(x => x.Contact));

            return model;
        }

        public override void Edit(NewAccountModel obj)
        {
            NewAccountDAL dal = new NewAccountDAL();
            INewAccount bl = dal.GetById(obj.Id);
            bl.AccountName = obj.AccountName;
            if (obj.Type == "Client")
                bl.AccountId = ClientRepository.GetByName(obj.AccountName);
            if (obj.Type == "Supplier")
                bl.AccountId = SupplierRepository.GetByName(obj.AccountName);
            bl.Type = obj.Type;
            bl.Date = obj.Date;
            bl.Description = obj.Description;
            bl.Phone = obj.Phone;
            bl.Street = obj.Street;
            bl.City = obj.City;
            bl.State = obj.State;
            bl.Fax = obj.Fax;
            bl.ZipPostalCode = obj.ZipPostalCode;
            bl.Website = obj.Website;
            bl.Email = obj.Email;
            IEmployee emp = new Employee { Id = obj.Empid };
            bl.Modifyby = emp;
            bl.ModifyDate = obj.ModifyDate;
            IIndustry ind = new Industry { Id = obj.IndustryId };
            ICountry cont = new Country { Id = obj.CountryId };
            bl.Industry = ind;
            bl.Country = cont;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(NewAccountModel obj)
        {
            NewAccountDAL dal = new NewAccountDAL();
            INewAccount bl = new NewAccount();
            bl.Date = obj.Date;
            bl.AccountName = obj.AccountName;
            if (obj.Type == "Client")
                bl.AccountId = ClientRepository.GetByName(obj.AccountName);
            if (obj.Type == "Supplier")
                bl.AccountId = SupplierRepository.GetByName(obj.AccountName);
            bl.Type = obj.Type;
            bl.Description = obj.Description;
            bl.Phone = obj.Phone;
            bl.Street = obj.Street;
            bl.City = obj.City;
            bl.State = obj.State;
            bl.ZipPostalCode = obj.ZipPostalCode;
            bl.Fax = obj.Fax;
            bl.Website = obj.Website;
            bl.Email = obj.Email;
            IEmployee e = new Employee { Id = obj.Empid };
            bl.Owner = e;
            IIndustry ind = new Industry { Id = obj.IndustryId };
            ICountry cont = new Country { Id = obj.CountryId };
            bl.Industry = ind;
            bl.Country = cont;

            dal.InsertOrUpdate(bl);

        }

        public void InsertBulk(List<NewAccountModel> model)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AccountTypeDAL a = new AccountTypeDAL();

            List<INewAccount> list = new List<INewAccount>();

            foreach (var obj in model.Where(x => x.dup != 1))
            {
                List<IAccountType> act = new List<IAccountType>();
                foreach (var item in obj.ActType)
                {
                    IAccountType t = a.GetById(item.Id);
                    act.Add(t);
                }
                INewAccount bl = new NewAccount();
                bl.Date = obj.Date;
                bl.AccountName = obj.AccountName;
                bl.AccountId = obj.AccountId;
                bl.Type = obj.Type;
                bl.Description = obj.Description;
                bl.Website = obj.Website;
                bl.Phone = obj.Phone;
                bl.Street = obj.Street;
                bl.City = obj.City;
                bl.State = obj.State;
                bl.ZipPostalCode = obj.ZipPostalCode;
                bl.Fax = obj.Fax;
                bl.Email = obj.Email;
                bl.Owner = obj.Owner;
                bl.Industry = obj.Industry;
                bl.Country = obj.Country;
                bl.ActType = act;
                list.Add(bl);
            }
            dal.InsertBulk(list);

        }

        public override bool Delete(int id)
        {
            NewAccountDAL dal = new NewAccountDAL();
            INewAccount bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        #region -- Contact Details ---

        public void AddContact(ContactModel obj, int id)
        {
            NewAccountDAL dal = new NewAccountDAL();
            ContactTypeDAL a = new ContactTypeDAL();
            IContact bl = new Contact();
            bl.Salutation = obj.Salutation;
            bl.FirstName = obj.FirstName;
            bl.MiddleName = obj.MiddleName;
            bl.LastName = obj.LastName;
            bl.Title = obj.Title;
            bl.Email = obj.Email;
            bl.Phone = obj.Phone;
            bl.PhoneDirect = obj.PhoneDirect;
            bl.Mobile = obj.Mobile;
            bl.Fax = obj.Fax;
            bl.Messanger = obj.Messanger;
            bl.EmailOpt = obj.EmailOpt;
            bl.Department = obj.Department;
            bl.Street = obj.Street;
            bl.City = obj.City;
            bl.StateProvince = obj.StateProvince;
            bl.ZipPostalCode = obj.ZipPostalCode;

            ICountry c = new Country { Id = obj.CountryId };
            bl.Country = c;

            if (obj.ContType != null)
            {
                List<IContactType> act = new List<IContactType>();
                foreach (var item in obj.ContType)
                {
                    IContactType t = a.GetById(item.Id);
                    act.Add(t);
                }
                bl.ContType = act;
            }
            IEmployee emp = new Employee { Id = obj.CreatorId };
            bl.Creater = emp;
            bl.CreationDate = obj.CreationDate;
            dal.AddContact(bl, id);

        }

        public IList<ContactModel> ContactList(int accountid)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetById(accountid).Contact);

            return model;
        }

        public IList<ContactModel> ContactListByType(string type)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(
                dal.GetAll().Where(x => x.Type == type).SelectMany(y => y.Contact));

            return model;
        }

        public IList<ContactModel> ContactList(string name)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByName(name));

            return model;
        }

        public bool DeleteContact(int id)
        {
            ContactDAL dal = new ContactDAL();
            IContact bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        #endregion

        public IList<NBOModel> GetAccountFiles(int id)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            List<NBOModel> model = AutoMapper.Mapper.Map<List<NBOModel>>(dal.GetAccountFiles(id));

            return model;
        }

        public static void AddActType(int id, string typeList)
        {
            NewAccountDAL dal = new NewAccountDAL();
            dal.AddAccountType(id, typeList);
        }

        public void DeleteActType(int id, int actid)
        {
            NewAccountDAL dal = new NewAccountDAL();
            AccountTypeDAL ct = new AccountTypeDAL();
            IAccountType c = ct.GetById(id);
            INewAccount bl = dal.GetById(actid);

            int i = bl.ActType.IndexOf(c);
            bl.ActType.RemoveAt(i);
            dal.InsertOrUpdate(bl);


        }

        public static void AddMailLog(AccountMailModel obj)
        {
            AccountMailLogDAL dal = new AccountMailLogDAL();
            IAccountMailLog bl = new AccountMailLog();
            ContactDAL ctdal = new ContactDAL();
            bl.CC = obj.CC;
            bl.Subject = obj.Subject;
            bl.Mailbody = obj.MailBody;
            bl.Date = MyExtension.UAETime();
            IEmployee emp = new Employee { Id = obj.OwnerId };
            bl.Owner = emp;
            INewAccount act = new NewAccount { Id = obj.Accountid };
            bl.AccountId = act;
            string[] contlist = obj.Idlist.Split(';');
            List<IContact> list = new List<IContact>();
            foreach (var item in contlist)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    IContact c = ctdal.GetById(Convert.ToInt32(item));
                    list.Add(c);
                }
            }
            bl.ContactList = list;
            dal.InsertOrUpdate(bl);

        }

    }

    public class AccountMailModel
    {
        public int Accountid { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Replyto { get; set; }
        public string Subject { get; set; }
        public string MailBody { get; set; }
        public string Idlist { get; set; }
        public int OwnerId { get; set; }
        public string type { get; set; }
    }
}