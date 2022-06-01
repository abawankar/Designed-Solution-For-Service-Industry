
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.ATM.Models.Masters
{
    public class LocationModel : Location
    {
    }

    public class LocationRepository : Repository<LocationModel>
    {

        public override LocationModel GetById(int id)
        {
            LocationDAL dal = new LocationDAL();
            AutoMapper.Mapper.CreateMap<Location, LocationModel>();
            LocationModel model = AutoMapper.Mapper.Map<LocationModel>(dal.GetById(id));
            return model;
        }

        public override System.Collections.Generic.IList<LocationModel> GetAll()
        {
            LocationDAL dal = new LocationDAL();
            AutoMapper.Mapper.CreateMap<Location, LocationModel>();
            List<LocationModel> model = AutoMapper.Mapper.Map<List<LocationModel>>(dal.GetAll());
            return model;
        }

        public override void Edit(LocationModel obj)
        {
            LocationDAL dal = new LocationDAL();
            ILocation bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(LocationModel obj)
        {
            LocationDAL dal = new LocationDAL();
            ILocation bl = new Location();
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            LocationDAL dal = new LocationDAL();
            ILocation bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}