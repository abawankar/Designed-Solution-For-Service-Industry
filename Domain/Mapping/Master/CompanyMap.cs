using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Master.Mapping
{
    public sealed class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Code);
            Map(x => x.Name);
            Map(x => x.Address);
            Map(x => x.PhoneNo);
            Map(x => x.FaxNo);
            HasMany<Branch>(x => x.Branches).KeyColumn("CompId").Cascade.All().LazyLoad();
        }
    }
}
