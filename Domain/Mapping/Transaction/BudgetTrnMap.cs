using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public class BudgetTrnMap : ClassMap<BudgetTrn>
    {
        public BudgetTrnMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.BudgetMonth);
            Map(x => x.Month);
            Map(x => x.ContractValue).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.ContractCost).CustomSqlType("numeric(12, 2)").Not.Nullable();
        }
    }
}
