using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public class PotentialAccountMap : ClassMap<PotentialAccount>
    {
        public PotentialAccountMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.AccountName);
            Map(x => x.AccountId);
            Map(x => x.Type);
            Map(x => x.Website);
            Map(x => x.Description).CustomSqlType("ntext").Nullable();
            Map(x => x.Phone);
            Map(x => x.Street);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.ZipPostalCode);
            References<Industry>(x => x.Industry).Column("IndustryId").LazyLoad();
            References<Country>(x => x.Country).Column("CountryId").LazyLoad();
            HasMany<PotentialContact>(x => x.Contact).KeyColumn("AccountId").Cascade.All().LazyLoad();
        }
    }
}
