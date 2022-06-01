using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public sealed class BranchMap : ClassMap<Branch>
    {
        public BranchMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Code);
            Map(x => x.Name);
            Map(x => x.Address);
            Map(x => x.PhoneNo);
            Map(x => x.FaxNo);
            HasMany<Department>(x => x.Departments).KeyColumn("BranchId").Cascade.All().LazyLoad();
            HasManyToMany<BusinessNature>(x => x.Nature).ParentKeyColumn("BranchId").ChildKeyColumn("NatureId").Cascade.All();
        }
    }
}
