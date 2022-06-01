using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public sealed class ReceivableMap : ClassMap<Receivable>
    {
        public ReceivableMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.DueDate);
            Map(x => x.Amount).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.DepositType);
            Map(x => x.Description);
            Map(x => x.DateReceived);
            Map(x => x.AmountReceived).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.PaymentMode);
            Map(x => x.Status);
            References<NBO>(x => x.NBO).Column("NboId").ForeignKey("fk_receivable_nboid").LazyLoad();
        }
    }
}
