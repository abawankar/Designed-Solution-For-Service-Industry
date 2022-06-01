using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class IndustryModel : Domain.Implementation.Master.Industry
    {
    }

    public class IndustryRepository : Repository<IndustryModel>
    {

        public override IndustryModel GetById(int id)
        {
            IndustryDAL dal = new IndustryDAL();
            AutoMapper.Mapper.CreateMap<Industry, IndustryModel>();
            IndustryModel model = AutoMapper.Mapper.Map<IndustryModel>(dal.GetById(id));

            return model;
        }

        public override IList<IndustryModel> GetAll()
        {
            IndustryDAL dal = new IndustryDAL();
            AutoMapper.Mapper.CreateMap<Industry, IndustryModel>();
            List<IndustryModel> model = AutoMapper.Mapper.Map<List<IndustryModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(IndustryModel obj)
        {
            IndustryDAL dal = new IndustryDAL();
            IIndustry bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(IndustryModel obj)
        {
            IndustryDAL dal = new IndustryDAL();
            IIndustry bl = new Industry();
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            IndustryDAL dal = new IndustryDAL();
            IIndustry bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}