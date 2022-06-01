using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public sealed class DepartmentMap : ClassMap<Department>
    {
        public DepartmentMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Code);
            Map(x => x.Name);
        }
    }
}
