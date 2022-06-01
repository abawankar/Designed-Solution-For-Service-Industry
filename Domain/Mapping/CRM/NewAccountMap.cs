using Domain.Implementation;
using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public class NewAccountMap : ClassMap<NewAccount>
    {
        public NewAccountMap()
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
            Map(x => x.Fax);
            Map(x => x.Email);
            References<Industry>(x => x.Industry).Column("IndustryId").ForeignKey("fk_account_indid").LazyLoad();
            References<Country>(x => x.Country).Column("CountryId").ForeignKey("fk_country_indid").LazyLoad();
            HasMany<Contact>(x => x.Contact).KeyColumn("AccountId").Cascade.All().LazyLoad();
            References<Employee>(x => x.Owner).Column("Owner").LazyLoad();
            Map(x => x.Date);
            HasManyToMany<AccountType>(x => x.ActType).Table("TypeOfAccount").Cascade.All().LazyLoad();
            Map(x => x.ModifyDate);
            References<Employee>(x => x.Modifyby).Column("Modifier").LazyLoad();
        }
    }
}
