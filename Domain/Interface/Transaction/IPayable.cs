using System;
using Domain.Interface.Master;

namespace Domain.Interface.Transaction
{
    public interface IPayable
    {
        int Id { get; set; }
        ISupplier PayingTo { get; set; }
        DateTime? DueDate { get; set; }
        double Amount { get; set; }
        string DepositType { get; set; }
        string Description { get; set; }
        DateTime? DatePaid { get; set; }
        double AmountPaid { get; set; }
        string PaymentMode { get; set; }
        int Status { get; set; }
        INBO NBO { get; set; }
    }
}
