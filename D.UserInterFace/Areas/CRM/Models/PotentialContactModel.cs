
using System.Collections.Generic;
using DAL.CRM;
using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class PotentialContactModel : Domain.Implementation.CRM.PotentialContact
    {
        public int CountryId { get; set; }
        public int Accountid { get; set; }
    }

    public class PotentialContactRepository : Repository<PotentialContactModel>
    {

        public override PotentialContactModel GetById(int id)
        {
            PotentialContactDAL dal = new PotentialContactDAL();
            AutoMapper.Mapper.CreateMap<PotentialContact, PotentialContactModel>();
            AutoMapper.Mapper.CreateMap<PotentialContact, PotentialContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            PotentialContactModel model = AutoMapper.Mapper.Map<PotentialContactModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<PotentialContactModel> GetAll()
        {
            PotentialContactDAL dal = new PotentialContactDAL();
            AutoMapper.Mapper.CreateMap<PotentialContact, PotentialContactModel>();
            AutoMapper.Mapper.CreateMap<PotentialContact, PotentialContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<PotentialContactModel> model = AutoMapper.Mapper.Map<List<PotentialContactModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(PotentialContactModel obj)
        {
            PotentialContactDAL dal = new PotentialContactDAL();
            IPotentialContact bl = dal.GetById(obj.Id);
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
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(PotentialContactModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override bool Delete(int id)
        {
            PotentialContactDAL dal = new PotentialContactDAL();
            IPotentialContact bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}