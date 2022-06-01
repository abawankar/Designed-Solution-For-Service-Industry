using System;
using Domain.Interface.Master;
using Domain.Interface.Transaction;

namespace Domain.Implementation.Transaction
{
    public class Payable : IPayable
    {
        public virtual int Id { get; set; }
        public virtual ISupplier PayingTo { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual double Amount { get; set; }
        public virtual string DepositType { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime? DatePaid { get; set; }
        public virtual double AmountPaid { get; set; }
        public virtual double? Balance
        {
            get
            {
                return Amount - AmountPaid;
            }
        }
        public virtual string PaymentMode { get; set; }
        public virtual int Status { get; set; }

        public virtual INBO NBO { get; set; }
    }
}
