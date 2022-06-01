using Domain.Implementation;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public sealed class AccountTypeMap : ClassMap<AccountType>
    {
        public AccountTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
        }
    }
}
