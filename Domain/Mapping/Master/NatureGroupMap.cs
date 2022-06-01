using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public class NatureGroupMap : ClassMap<NatureGroup>
    {
        public NatureGroupMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            HasManyToMany<BusinessNature>(x => x.NatureName).Table("BusinessNatureGroup").Cascade.All();
        }
    }
}
