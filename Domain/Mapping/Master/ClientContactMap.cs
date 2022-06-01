using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public sealed class ClientContactMap : ClassMap<ClientContact>
    {
        public ClientContactMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.ContactName);
            Map(x => x.EmailId);
            Map(x => x.Phone);
            Map(x => x.Mobile);
            Map(x => x.Fax);
        }
    }
}
