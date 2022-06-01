using System;
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Master
{
    public class EnquiryStatusModel : Domain.Implementation.Master.EnquiryStatus
    {
    }

    public class EnquiryStatusRepository : Repository<EnquiryStatusModel>
    {

        public override EnquiryStatusModel GetById(int id)
        {
            EnquiryStatusDAL dal = new EnquiryStatusDAL();
            AutoMapper.Mapper.CreateMap<EnquiryStatus, EnquiryStatusModel>();
            EnquiryStatusModel model = AutoMapper.Mapper.Map<EnquiryStatusModel>(dal.GetById(id));

            return model;
        }

        public override IList<EnquiryStatusModel> GetAll()
        {
            EnquiryStatusDAL dal = new EnquiryStatusDAL();
            AutoMapper.Mapper.CreateMap<EnquiryStatus, EnquiryStatusModel>();
            List<EnquiryStatusModel> model = AutoMapper.Mapper.Map<List<EnquiryStatusModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(EnquiryStatusModel obj)
        {
            EnquiryStatusDAL dal = new EnquiryStatusDAL();
            IEnquiryStatus bl = dal.GetById(obj.Id);
            bl.Name = obj.Name.ToUpper();
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(EnquiryStatusModel obj)
        {
            EnquiryStatusDAL dal = new EnquiryStatusDAL();
            IEnquiryStatus bl = new EnquiryStatus();
            bl.Name = obj.Name.ToUpper();
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}