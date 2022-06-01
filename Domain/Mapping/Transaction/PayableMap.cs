using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public sealed class PayableMap : ClassMap<Payable>
    {
        public PayableMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            References<Supplier>(x => x.PayingTo).Column("SuppId").ForeignKey("fk_payable_suppid").LazyLoad();
            Map(x => x.DueDate);
            Map(x => x.Amount).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.DepositType);
            Map(x => x.Description);
            Map(x => x.DatePaid);
            Map(x => x.AmountPaid).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.PaymentMode);
            Map(x => x.Status);
            References<NBO>(x => x.NBO).Column("NboId").ForeignKey("fk_payable_nboid").LazyLoad();
        }
    }
}
