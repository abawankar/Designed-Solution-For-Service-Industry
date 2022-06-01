using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public class SupplierMap : ClassMap<Supplier>
    {
        public SupplierMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.AVLCode);
            References<SupplierCategory>(x => x.Category).Column("CatId").ForeignKey("fk_supplier_cat").LazyLoad();
        }
    }
}
