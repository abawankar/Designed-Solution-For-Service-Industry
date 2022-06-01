using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public class BudgetMap : ClassMap<Budget>
    {
        public BudgetMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Year);
            References<Employee>(x => x.Employee).Column("EmpId").ForeignKey("fk_budget_EmpId").LazyLoad();
            References<BusinessNature>(x => x.Nature).Column("NatureId").ForeignKey("fk_budget_natureid").LazyLoad();
            HasMany<BudgetTrn>(x => x.BudTrn).KeyColumn("BudgetId").Cascade.All().LazyLoad();
            References<Branch>(x => x.Branch).Column("BranchId").ForeignKey("fk_budget_branchid").LazyLoad();

        }
    }
}
