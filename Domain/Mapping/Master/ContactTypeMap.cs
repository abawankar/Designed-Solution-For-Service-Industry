using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public class ContactTypeMap : ClassMap<ContactType>
    {
        public ContactTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
        }
    }
}
