using System.Collections.Generic;
using System.Linq;
using DAL.Transaction;
using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using Domain.Interface.Master;
using Domain.Interface.Transaction;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Transaction
{
    public class PayableModel : Domain.Implementation.Transaction.Payable
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int NboId { get; set; }
    }

    public class PayableRepository : Repository<PayableModel>
    {

        public override void Edit(PayableModel obj)
        {
            PayableDAL dal = new PayableDAL();
            IPayable bl = dal.GetById(obj.Id);
            bl.DueDate = obj.DueDate;
            bl.Amount = obj.Amount;
            bl.DepositType = obj.DepositType;
            bl.Description = obj.Description;
            bl.DatePaid = obj.DatePaid;
            bl.AmountPaid = obj.AmountPaid;
            bl.Status = obj.Status;

            ISupplier s = new Supplier { Id = obj.SupplierId };
            bl.PayingTo = s;

            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            PayableDAL dal = new PayableDAL();
            IPayable bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        public IList<PayableModel> GetPayable(int nboid)
        {
            PayableDAL dal = new PayableDAL();
            AutoMapper.Mapper.CreateMap<Payable, PayableModel>();
            AutoMapper.Mapper.CreateMap<Payable, PayableModel>()
                .ForMember(dest => dest.NboId, opt => opt.MapFrom(scr => scr.NBO.Id))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(scr => scr.PayingTo.Id));
            List<PayableModel> model = AutoMapper.Mapper.Map<List<PayableModel>>(dal.GetAll().Where(x => x.NBO.Id == nboid));

            return model;
        }

        public void InsertPayable(PayableModel obj, int nboid)
        {
            PayableDAL dal = new PayableDAL();

            IPayable bl = new Payable();
            bl.DueDate = obj.DueDate;
            bl.Amount = obj.Amount;
            bl.DepositType = obj.DepositType;
            bl.Description = obj.Description;
            bl.DatePaid = obj.DatePaid;
            bl.AmountPaid = obj.AmountPaid;
            bl.Status = obj.Status;

            ISupplier s = new Supplier { Id = obj.SupplierId };
            bl.PayingTo = s;

            INBO nbo = new NBO { Id = nboid };
            bl.NBO = nbo;

            dal.InsertOrUpdate(bl);

        }

        public override PayableModel GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public override IList<PayableModel> GetAll()
        {
            PayableDAL dal = new PayableDAL();
            AutoMapper.Mapper.CreateMap<Payable, PayableModel>();
            AutoMapper.Mapper.CreateMap<Payable, PayableModel>()
                .ForMember(dest => dest.NboId, opt => opt.MapFrom(scr => scr.NBO.Id))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(scr => scr.PayingTo.Name))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(scr => scr.PayingTo.Id));
            List<PayableModel> model = AutoMapper.Mapper.Map<List<PayableModel>>(dal.GetAll());

            return model;
        }

        public override void Insert(PayableModel obj)
        {
            throw new System.NotImplementedException();
        }
    }
}