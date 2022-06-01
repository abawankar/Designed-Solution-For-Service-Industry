using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    class SupplierCategoryMap : ClassMap<SupplierCategory>
    {
        public SupplierCategoryMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
        }
    }
}
