using System.Collections.Generic;
using System.Linq;
using DAL.Transaction;
using Domain.Implementation.Transaction;
using Domain.Interface.Transaction;
using D.UserInterFace.Models;
using System;

namespace D.UserInterFace.Areas.SCM.Models.Transaction
{
    public class ReceivableModel : Domain.Implementation.Transaction.Receivable
    {
        public int NboId { get; set; }
        public string ClientName { get; set; }
    }

    public class ReceivableRepository : Repository<ReceivableModel>
    {
        public override void Edit(ReceivableModel obj)
        {
            ReceivableDAL dal = new ReceivableDAL();
            IReceivable bl = dal.GetById(obj.Id);
            bl.DueDate = obj.DueDate;
            bl.Amount = obj.Amount;
            bl.DepositType = obj.DepositType;
            bl.Description = obj.Description;
            bl.DateReceived = obj.DateReceived;
            bl.AmountReceived = obj.AmountReceived;
            bl.Status = obj.Status;

            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            ReceivableDAL dal = new ReceivableDAL();
            IReceivable bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        public void InsertReceivable(ReceivableModel obj, int nboid)
        {
            ReceivableDAL dal = new ReceivableDAL();

            IReceivable bl = new Receivable();
            bl.DueDate = obj.DueDate;
            bl.Amount = obj.Amount;
            bl.DepositType = obj.DepositType;
            bl.Description = obj.Description;
            bl.DateReceived = obj.DateReceived;
            bl.AmountReceived = obj.AmountReceived;
            bl.Status = obj.Status;

            INBO nbo = new NBO { Id = nboid };
            bl.NBO = nbo;

            dal.InsertOrUpdate(bl);
        }

        public IList<ReceivableModel> GetReceivable(int nboid)
        {
            ReceivableDAL dal = new ReceivableDAL();
            AutoMapper.Mapper.CreateMap<Receivable, ReceivableModel>();
            AutoMapper.Mapper.CreateMap<Receivable, ReceivableModel>()
                .ForMember(dest => dest.NboId, opt => opt.MapFrom(scr => scr.NBO.Id))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(scr => scr.NBO.ClientName.Name));
            List<ReceivableModel> model = AutoMapper.Mapper.Map<List<ReceivableModel>>(dal.GetAll().Where(x => x.NBO.Id == nboid));

            return model;
        }

        public override ReceivableModel GetById(int id)
        {
            ReceivableDAL dal = new ReceivableDAL();
            AutoMapper.Mapper.CreateMap<Receivable, ReceivableModel>();
            AutoMapper.Mapper.CreateMap<Receivable, ReceivableModel>()
                .ForMember(dest => dest.NboId, opt => opt.MapFrom(scr => scr.NBO.Id))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(scr => scr.NBO.ClientName.Name));
            ReceivableModel model = AutoMapper.Mapper.Map<ReceivableModel>(dal.GetById(id));

            return model;
        }

        public override IList<ReceivableModel> GetAll()
        {
            List<ReceivableModel> model = new List<ReceivableModel>();
            try
            {
                ReceivableDAL dal = new ReceivableDAL();
                AutoMapper.Mapper.CreateMap<Receivable, ReceivableModel>();
                AutoMapper.Mapper.CreateMap<Receivable, ReceivableModel>()
                    .ForMember(dest => dest.NboId, opt => opt.MapFrom(scr => scr.NBO.Id))
                    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(scr => scr.NBO.ClientName.Name));
                model = AutoMapper.Mapper.Map<List<ReceivableModel>>(dal.GetAll());

            }
            catch (Exception ex) { throw ex; }
            return model;
        }

        public override void Insert(ReceivableModel obj)
        {
            throw new System.NotImplementedException();
        }

        public static double Received(int nboid)
        {
            ReceivableRepository dal = new ReceivableRepository();
            var data = from m in dal.GetAll().Where(x => x.NBO.Id == nboid)
                       group m by m.NBO.Id into g
                       select new { received = g.Sum(x => x.AmountReceived) };
            return data.Select(x => x.received).SingleOrDefault();
        }
    }

}