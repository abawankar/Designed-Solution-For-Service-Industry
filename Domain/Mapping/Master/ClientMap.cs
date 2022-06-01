using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public class ClientMap : ClassMap<Client>
    {
        public ClientMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.ClientGroup);
            Map(x => x.Remarks);
            References<Country>(x => x.Country).Column("CountryId").ForeignKey("fk_client_countryid").LazyLoad();

        }
    }
}
