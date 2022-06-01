
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
    public class PotentialAccountModel : Domain.Implementation.CRM.PotentialAccount
    {
        public int IndustryId { get; set; }
        public int CountryId { get; set; }
    }

    public class PotentialAccountRepository : Repository<PotentialAccountModel>
    {

        public override PotentialAccountModel GetById(int id)
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();
            AutoMapper.Mapper.CreateMap<PotentialAccount, PotentialAccountModel>();
            AutoMapper.Mapper.CreateMap<PotentialAccount, PotentialAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            PotentialAccountModel model = AutoMapper.Mapper.Map<PotentialAccountModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<PotentialAccountModel> GetAll()
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();
            AutoMapper.Mapper.CreateMap<PotentialAccount, PotentialAccountModel>();
            AutoMapper.Mapper.CreateMap<PotentialAccount, PotentialAccountModel>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(scr => scr.Industry.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<PotentialAccountModel> model = AutoMapper.Mapper.Map<List<PotentialAccountModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(PotentialAccountModel obj)
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();
            IPotentialAccount bl = dal.GetById(obj.Id);
            bl.AccountName = obj.AccountName;
            bl.AccountId = 0;
            bl.Type = obj.Type;
            bl.Description = obj.Description;
            bl.Phone = obj.Phone;
            bl.Street = obj.Street;
            bl.City = obj.City;
            bl.State = obj.State;
            bl.ZipPostalCode = obj.ZipPostalCode;

            IIndustry ind = new Industry { Id = obj.IndustryId };
            ICountry cont = new Country { Id = obj.CountryId };
            bl.Industry = ind;
            bl.Country = cont;

            dal.InsertOrUpdate(bl);
        }

        public override void Insert(PotentialAccountModel obj)
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();
            IPotentialAccount bl = new PotentialAccount();
            bl.AccountName = obj.AccountName;
            bl.AccountId = 0;
            bl.Type = obj.Type;
            bl.Website = obj.Website;
            bl.Description = obj.Description;
            bl.Phone = obj.Phone;
            bl.Street = obj.Street;
            bl.City = obj.City;
            bl.State = obj.State;
            bl.ZipPostalCode = obj.ZipPostalCode;
            IIndustry ind = new Industry { Id = obj.IndustryId };
            ICountry cont = new Country { Id = obj.CountryId };
            bl.Industry = ind;
            bl.Country = cont;
            dal.InsertOrUpdate(bl);
        }

        public void InsertBulk(List<PotentialAccountModel> model)
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();

            List<IPotentialAccount> list = new List<IPotentialAccount>();

            foreach (var obj in model)
            {
                IPotentialAccount bl = new PotentialAccount();
                bl.AccountName = obj.AccountName;
                bl.AccountId = 0;
                bl.Type = obj.Type;
                bl.Website = obj.Website;
                bl.Description = obj.Description;
                bl.Phone = obj.Phone;
                bl.Street = obj.Street;
                bl.City = obj.City;
                bl.State = obj.State;
                bl.Industry = obj.Industry;
                bl.Country = obj.Country;
                list.Add(bl);
            }
            dal.InsertBulk(list);

        }

        public override bool Delete(int id)
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();
            IPotentialAccount bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        public bool Convert(PotentialAccountModel obj, int empid)
        {
            try
            {
                ClientDAL client = new ClientDAL();
                SupplierDAL sup = new SupplierDAL();
                if (obj.Type == "Client")
                {
                    var data = client.GetByNameList(obj.AccountName).Where(x => x.Country.Id == obj.Country.Id);
                    if (data.Count() > 0)
                    {
                        obj.AccountId = data.Select(x => x.Id).Single();
                    }
                    else
                    {
                        IClient c = new Client();
                        c.Name = obj.AccountName;
                        ICountry ct = new Country { Id = obj.Country.Id };
                        c.Country = ct;
                        client.InsertOrUpdate(c);

                        c = client.GetByNameList(obj.AccountName).Where(x => x.Country.Id == obj.Country.Id).Single();
                        obj.AccountId = c.Id;
                    }


                }
                if (obj.Type == "Supplier")
                {
                    ISupplier s = new Supplier();
                    s.Name = obj.AccountName;
                    ISupplierCategory sc = new SupplierCategory { Id = 1 };
                    s.Category = sc;
                    sup.InsertOrUpdate(s);

                    s = sup.GetByName(obj.AccountName);
                    obj.AccountId = s.Id;
                }

                NewAccountDAL dal = new NewAccountDAL();
                INewAccount bl = new NewAccount();
                bl.Date = MyExtension.UAETime();
                bl.AccountId = obj.AccountId;
                bl.AccountName = obj.AccountName;
                bl.Type = obj.Type;
                bl.Description = obj.Description;
                bl.Phone = obj.Phone;
                bl.Street = obj.Street;
                bl.City = obj.City;
                bl.State = obj.State;
                bl.ZipPostalCode = obj.ZipPostalCode;
                bl.Website = obj.Website;
                IIndustry ind = new Industry { Id = obj.IndustryId };
                ICountry cont = new Country { Id = obj.CountryId };
                bl.Industry = ind;
                bl.Country = cont;

                IEmployee e = new Employee { Id = empid };
                bl.Owner = e;

                List<IContact> contactList = new List<IContact>();
                foreach (var item in obj.Contact)
                {
                    IContact ncont = new Contact();
                    ncont.Salutation = item.Salutation;
                    ncont.FirstName = item.FirstName;
                    ncont.MiddleName = item.MiddleName;
                    ncont.LastName = item.LastName;
                    ncont.Title = item.Title;
                    ncont.Email = item.Email;
                    ncont.Phone = item.Phone;
                    ncont.PhoneDirect = item.PhoneDirect;
                    ncont.Mobile = item.Mobile;
                    ncont.Fax = item.Fax;
                    ncont.Messanger = item.Messanger;
                    ncont.EmailOpt = item.EmailOpt;
                    ncont.Department = item.Department;
                    ncont.Street = obj.Street;
                    ncont.City = obj.City;
                    ncont.StateProvince = item.StateProvince;
                    ncont.ZipPostalCode = item.ZipPostalCode;
                    ncont.AccountName = obj.AccountName;
                    ICountry conta = new Country { Id = obj.CountryId };
                    ncont.Country = conta;
                    contactList.Add(ncont);
                }
                bl.Contact = contactList;
                dal.InsertOrUpdate(bl);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region -- Contact Details ---

        public void AddContact(PotentialContactModel obj, int id)
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();
            IPotentialContact bl = new PotentialContact();
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
            bl.Country = obj.Country;
            dal.AddContact(bl, id);

        }

        public IList<PotentialContactModel> ContactList(int accountid)
        {
            PotentialAccountDAL dal = new PotentialAccountDAL();
            AutoMapper.Mapper.CreateMap<PotentialContact, PotentialContactModel>();
            AutoMapper.Mapper.CreateMap<PotentialContact, PotentialContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<PotentialContactModel> model = AutoMapper.Mapper.Map<List<PotentialContactModel>>(dal.GetById(accountid).Contact);

            return model;
        }

        public bool DeleteContact(int id)
        {
            PotentialContactDAL dal = new PotentialContactDAL();
            IPotentialContact bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        #endregion
    }
}