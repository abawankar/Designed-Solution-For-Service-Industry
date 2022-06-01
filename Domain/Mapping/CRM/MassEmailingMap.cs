using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public sealed class MassEmailingMap : ClassMap<MassEmailing>
    {
        public MassEmailingMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            HasManyToMany<Contact>(x => x.ContactGroup).Table("ContactMassEmail").Cascade.All().LazyLoad();
            References<EmailTemplate>(x => x.EmailTemplate).LazyLoad();
            References<Employee>(x => x.Owner).LazyLoad();
            Map(x => x.Name);
            Map(x => x.Schedule);
            Map(x => x.TimeZone);
            Map(x => x.ServerTime);
            Map(x => x.Status);
            Map(x => x.Bcc);
            Map(x => x.ReplyTo);
            Map(x => x.Salutation);
        }
    }
}
