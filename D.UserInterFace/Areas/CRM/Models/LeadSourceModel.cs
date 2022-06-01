using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class LeadSourceModel : Domain.Implementation.Master.LeadSource
    {
    }

    public class LeadSourceRepository : Repository<LeadSourceModel>
    {

        public override LeadSourceModel GetById(int id)
        {
            LeadSourceDAL dal = new LeadSourceDAL();
            AutoMapper.Mapper.CreateMap<LeadSource, LeadSourceModel>();
            LeadSourceModel model = AutoMapper.Mapper.Map<LeadSourceModel>(dal.GetById(id));
            return model;
        }

        public override IList<LeadSourceModel> GetAll()
        {
            LeadSourceDAL dal = new LeadSourceDAL();
            AutoMapper.Mapper.CreateMap<LeadSource, LeadSourceModel>();
            List<LeadSourceModel> model = AutoMapper.Mapper.Map<List<LeadSourceModel>>(dal.GetAll());
            return model;
        }

        public override void Edit(LeadSourceModel obj)
        {
            LeadSourceDAL dal = new LeadSourceDAL();
            ILeadSource bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(LeadSourceModel obj)
        {
            LeadSourceDAL dal = new LeadSourceDAL();
            ILeadSource bl = new LeadSource();
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            LeadSourceDAL dal = new LeadSourceDAL();
            ILeadSource bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}