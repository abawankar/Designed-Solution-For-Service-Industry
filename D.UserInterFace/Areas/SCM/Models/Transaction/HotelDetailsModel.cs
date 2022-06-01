using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Transaction;
using Domain.Implementation.Transaction;
using Domain.Interface.Transaction;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Transaction
{
    public class HotelDetailsModel : Domain.Implementation.Transaction.HotelDetails
    {
    }

    public class HotelDetailsRepository : Repository<HotelDetailsModel>
    {
        public override HotelDetailsModel GetById(int id)
        {
            HotelDetailsDAL dal = new HotelDetailsDAL();
            AutoMapper.Mapper.CreateMap<HotelDetails, HotelDetailsModel>();
            HotelDetailsModel model = AutoMapper.Mapper.Map<HotelDetailsModel>(dal.GetById(id));

            return model;
        }

        public override IList<HotelDetailsModel> GetAll()
        {
            HotelDetailsDAL dal = new HotelDetailsDAL();
            AutoMapper.Mapper.CreateMap<HotelDetails, HotelDetailsModel>();
            List<HotelDetailsModel> model = AutoMapper.Mapper.Map<List<HotelDetailsModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(HotelDetailsModel obj)
        {
            HotelDetailsDAL dal = new HotelDetailsDAL();
            IHotelDetails bl = dal.GetById(obj.Id);
            bl.HotelName = obj.HotelName;
            bl.NumberOfNight = obj.NumberOfNight;
            bl.HotelBillingValue = obj.HotelBillingValue;
            dal.InsertOrUpdate(bl);

        }

        public override void Insert(HotelDetailsModel obj)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(int id)
        {
            HotelDetailsDAL dal = new HotelDetailsDAL();
            IHotelDetails bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        public IList<HotelDetailsModel> GetNBOHotel(int nboid)
        {
            HotelDetailsDAL dal = new HotelDetailsDAL();
            AutoMapper.Mapper.CreateMap<HotelDetails, HotelDetailsModel>();
            List<HotelDetailsModel> model = AutoMapper.Mapper.Map<List<HotelDetailsModel>>(dal.GetAll().Where(x => x.NBO.Id == nboid));

            return model;
        }

        public void AddHotel(HotelDetailsModel obj, int nboid)
        {
            HotelDetailsDAL dal = new HotelDetailsDAL();
            IHotelDetails bl = new HotelDetails();
            bl.HotelName = obj.HotelName;
            bl.NumberOfNight = obj.NumberOfNight;
            bl.HotelBillingValue = obj.HotelBillingValue;
            INBO nbo = new NBO { Id = nboid };
            bl.NBO = nbo;

            dal.InsertOrUpdate(bl);
        }
    }
}