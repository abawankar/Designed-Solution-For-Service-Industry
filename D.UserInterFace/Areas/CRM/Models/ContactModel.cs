
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.CRM;
using DAL.Master;
using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class ContactModel : Domain.Implementation.CRM.Contact
    {
        public int CountryId { get; set; }
        public int Accountid { get; set; }
        public int CreatorId { get; set; }
        public int ModifierId { get; set; }
    }

    public class ContactRepository : Repository<ContactModel>
    {
        public override ContactModel GetById(int id)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            ContactModel model = AutoMapper.Mapper.Map<ContactModel>(dal.GetById(id));

            return model;
        }

        public override IList<ContactModel> GetAll()
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetAll());

            return model;
        }

        public IList<ContactModel> GetTop()
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetTop());

            return model;
        }

        public IList<ContactModel> GetIdList(int id)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetIdList(id));
            return model;
        }

        public IList<Contact> GetCity()
        {
            ContactDAL dal = new ContactDAL();
            List<Contact> list = dal.GetCity().ToList();
            return list;
        }

        public override void Edit(ContactModel obj)
        {
            ContactDAL dal = new ContactDAL();
            IContact bl = dal.GetById(obj.Id);
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
            ICountry cont = new Country { Id = obj.CountryId };
            bl.Country = cont;
            IEmployee emp = new Employee { Id = obj.ModifierId };
            bl.Modifyby = emp;
            bl.ModifyDate = obj.ModifyDate;
            dal.InsertOrUpdate(bl);

        }

        public override void Insert(ContactModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override bool Delete(int id)
        {
            ContactDAL dal = new ContactDAL();
            IContact bl = dal.GetById(id);
            bl.ContType.Clear();
            return dal.DeleteContact(bl);
        }

        public IList<ContactModel> GetByGroupId(int id)
        {
            ContactGroupRepository dal = new ContactGroupRepository();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetById(id).ContactList);

            return model;
        }

        public IList<ContactModel> GetByCountryid(int id)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByCountryid(id));

            return model;
        }

        public IList<ContactModel> GetByContactType(int id)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByContactType(id));

            return model;
        }

        public IList<ContactModel> GetByCity(string city)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByCity(city));

            return model;
        }

        public IList<ContactModel> GetByFirstName(string fName)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByFirstName(fName));

            return model;
        }

        public IList<ContactModel> GetByLastName(string lName)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetByLastName(lName));

            return model;
        }

        public IList<ContactModel> GetBySearch(string find)
        {
            ContactDAL dal = new ContactDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetBySearch(find));

            return model;
        }

        public static void AddContType(int id, string typeList)
        {
            ContactDAL dal = new ContactDAL();
            dal.AddContactType(id, typeList);
        }

        public void DeleteContType(int id, int contId)
        {
            ContactDAL dal = new ContactDAL();
            ContactTypeDAL ct = new ContactTypeDAL();
            IContactType c = ct.GetById(id);
            IContact bl = dal.GetById(contId);

            int i = bl.ContType.IndexOf(c);
            bl.ContType.RemoveAt(i);
            dal.InsertOrUpdate(bl);


        }

        public void MergeContact(string bid, string uid)
        {
            try
            {
                ContactDAL dal = new ContactDAL();
                IContact bl = dal.GetById(Convert.ToInt32(bid));
                IContact obj = dal.GetById(Convert.ToInt32(uid));
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
                ICountry cont = new Country { Id = obj.Country.Id };
                bl.Country = cont;
                dal.InsertOrUpdate(bl);

                obj.ContType.Clear();
                var data = dal.Delete(obj);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}