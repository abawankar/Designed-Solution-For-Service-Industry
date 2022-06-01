using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public sealed class ContactGroupMap : ClassMap<ContactGroup>
    {
        public ContactGroupMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            References<Employee>(x => x.GroupOwner).LazyLoad();
            Map(x => x.Date);
            Map(x => x.Note);
            HasManyToMany<Contact>(x => x.ContactList).Table("GroupContactList").Cascade.All().LazyLoad();
        }
    }
}
