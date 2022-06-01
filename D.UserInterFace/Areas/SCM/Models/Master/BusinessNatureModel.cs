using System;
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Master
{
    public class BusinessNatureModel : Domain.Implementation.Master.BusinessNature
    {
    }

    public class BusinessNatureRepository : Repository<BusinessNatureModel>
    {

        public override BusinessNatureModel GetById(int id)
        {
            BusinessNatureDAL dal = new BusinessNatureDAL();
            AutoMapper.Mapper.CreateMap<BusinessNature, BusinessNatureModel>();
            BusinessNatureModel model = AutoMapper.Mapper.Map<BusinessNatureModel>(dal.GetById(id));

            return model;
        }

        public override IList<BusinessNatureModel> GetAll()
        {
            BusinessNatureDAL dal = new BusinessNatureDAL();
            AutoMapper.Mapper.CreateMap<BusinessNature, BusinessNatureModel>();
            List<BusinessNatureModel> model = AutoMapper.Mapper.Map<List<BusinessNatureModel>>(dal.GetAll());
            return model;
        }

        public IList<BusinessNatureModel> GetAllBySelect()
        {
            BusinessNatureDAL dal = new BusinessNatureDAL();
            AutoMapper.Mapper.CreateMap<BusinessNature, BusinessNatureModel>();
            List<BusinessNatureModel> model = AutoMapper.Mapper.Map<List<BusinessNatureModel>>(dal.GetAll());
            model.Add(new BusinessNatureModel { Id = 0, Name = "* Select *" });
            return model;
        }

        public override void Edit(BusinessNatureModel obj)
        {
            BusinessNatureDAL dal = new BusinessNatureDAL();
            IBusinessNature bl = dal.GetById(obj.Id);
            bl.Name = obj.Name.ToUpper();
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(BusinessNatureModel obj)
        {
            BusinessNatureDAL dal = new BusinessNatureDAL();
            IBusinessNature bl = new BusinessNature();
            bl.Name = obj.Name.ToUpper();
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public static string NatureName(int id)
        {
            if (id == 0)
            {
                return "All";
            }
            else
            {
                BusinessNatureRepository e = new BusinessNatureRepository();
                return e.GetById(id).Name;
            }
        }
    }
}