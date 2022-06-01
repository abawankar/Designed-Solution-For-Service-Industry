using System;
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;

namespace D.UserInterFace.Models.Masters
{
    public class NatureGroupModel : Domain.Implementation.Master.NatureGroup
    {
    }

    public class NatureGroupRepository : Repository<NatureGroupModel>
    {
        public override NatureGroupModel GetById(int id)
        {
            NatureGroupDAL dal = new NatureGroupDAL();
            AutoMapper.Mapper.CreateMap<Domain.Implementation.Master.NatureGroup, NatureGroupModel>();
            NatureGroupModel model = AutoMapper.Mapper.Map<NatureGroupModel>(dal.GetById(id));
            return model;
        }

        public override IList<NatureGroupModel> GetAll()
        {
            NatureGroupDAL dal = new NatureGroupDAL();
            AutoMapper.Mapper.CreateMap<Domain.Implementation.Master.NatureGroup, NatureGroupModel>();
            List<NatureGroupModel> model = AutoMapper.Mapper.Map<List<NatureGroupModel>>(dal.GetAll());
            return model;
        }

        public override void Edit(NatureGroupModel obj)
        {
            NatureGroupDAL dal = new NatureGroupDAL();
            INatureGroup bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(NatureGroupModel obj)
        {
            NatureGroupDAL dal = new NatureGroupDAL();
            INatureGroup bl = new NatureGroup();
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public static void AddBusinessNature(int groupid, string naturelist)
        {
            NatureGroupDAL dal = new NatureGroupDAL();
            dal.AddBusinessNature(groupid, naturelist);
        }
    }
}