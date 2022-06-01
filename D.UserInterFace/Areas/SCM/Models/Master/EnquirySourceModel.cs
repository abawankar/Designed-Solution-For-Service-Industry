using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Master
{
    public class EnquirySourceModel : Domain.Implementation.Master.EnquirySource
    {
    }

    public class EnquirySourceRepository : Repository<EnquirySourceModel>
    {

        public override EnquirySourceModel GetById(int id)
        {
            EnquirySourceDAL dal = new EnquirySourceDAL();
            AutoMapper.Mapper.CreateMap<EnquirySource, EnquirySourceModel>();
            EnquirySourceModel model = AutoMapper.Mapper.Map<EnquirySourceModel>(dal.GetById(id));

            return model;
        }

        public override IList<EnquirySourceModel> GetAll()
        {
            EnquirySourceDAL dal = new EnquirySourceDAL();
            AutoMapper.Mapper.CreateMap<EnquirySource, EnquirySourceModel>();
            List<EnquirySourceModel> model = AutoMapper.Mapper.Map<List<EnquirySourceModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(EnquirySourceModel obj)
        {
            EnquirySourceDAL dal = new EnquirySourceDAL();
            IEnquirySource bl = dal.GetById(obj.Id);
            bl.Name = obj.Name.ToUpper();
            bl.AppointmentDate = obj.AppointmentDate;
            bl.TerminationDate = obj.TerminationDate;
            bl.RetainerFee = obj.RetainerFee;
            bl.CommLeisure = obj.CommLeisure;
            bl.CommMice = obj.CommMice;
            bl.Active = obj.Active;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(EnquirySourceModel obj)
        {
            EnquirySourceDAL dal = new EnquirySourceDAL();
            IEnquirySource bl = new EnquirySource();
            bl.Name = obj.Name.ToUpper();
            bl.AppointmentDate = obj.AppointmentDate;
            bl.TerminationDate = obj.TerminationDate;
            bl.RetainerFee = obj.RetainerFee;
            bl.CommLeisure = obj.CommLeisure;
            bl.CommMice = obj.CommMice;
            bl.Active = obj.Active;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public static string Enquiry(string name)
        {
            EnquirySourceRepository c = new EnquirySourceRepository();
            string enquiry = "";
            if (string.IsNullOrEmpty(name))
            {
                return enquiry = "All";
            }
            else
            {
                string[] id = name.Split(',');
                foreach (var item in c.GetAll().Where(x => id.Contains(x.Id.ToString())))
                {
                    if (enquiry == "")
                    {
                        enquiry = item.Name;
                    }
                    else
                    {
                        enquiry = enquiry + "/" + item.Name;
                    }
                }
                return enquiry;
            }
        }
    }
}