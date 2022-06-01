using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public sealed class AccountMailLogMap : ClassMap<AccountMailLog>
    {
        public AccountMailLogMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            References<NewAccount>(x => x.AccountId).LazyLoad();
            HasManyToMany<Contact>(x => x.ContactList).Table("AccountMailContact").Cascade.All().LazyLoad();
            Map(x => x.CC);
            Map(x => x.Subject);
            Map(x => x.Mailbody).CustomSqlType("ntext").Nullable();
            References<Employee>(x => x.Owner).LazyLoad();
            Map(x => x.Date);


        }
    }
}
