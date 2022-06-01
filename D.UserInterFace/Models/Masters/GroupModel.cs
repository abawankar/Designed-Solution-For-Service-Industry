
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
namespace D.UserInterFace.Models.Masters
{
    public class GroupModel : Domain.Implementation.Master.Group
    {
    }

    public class GroupRepository : Repository<GroupModel>
    {

        public override GroupModel GetById(int id)
        {
            GroupDAL dal = new GroupDAL();
            AutoMapper.Mapper.CreateMap<Group, GroupModel>();
            GroupModel model = AutoMapper.Mapper.Map<GroupModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<GroupModel> GetAll()
        {
            GroupDAL dal = new GroupDAL();
            AutoMapper.Mapper.CreateMap<Group, GroupModel>();
            List<GroupModel> model = AutoMapper.Mapper.Map<List<GroupModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(GroupModel obj)
        {
            try
            {
                GroupDAL dal = new GroupDAL();
                IGroup bl = dal.GetById(obj.Id);
                bl.Name = obj.Name;
                dal.InsertOrUpdate(bl);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public override void Insert(GroupModel obj)
        {
            try
            {
                GroupDAL dal = new GroupDAL();
                IGroup bl = new Group();
                bl.Name = obj.Name;
                dal.InsertOrUpdate(bl);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}