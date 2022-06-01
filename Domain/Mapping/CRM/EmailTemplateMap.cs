using Domain.Implementation.CRM;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public class EmailTemplateMap : ClassMap<EmailTemplate>
    {
        public EmailTemplateMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.TemplateName);
            Map(x => x.UniqueName);
            Map(x => x.Encoding);
            Map(x => x.Description);
            Map(x => x.Subject);
            Map(x => x.EmailBody).CustomSqlType("ntext").Nullable();
            Map(x => x.Type);

            HasMany<EmailAttachment>(x => x.Attachment).Cascade.All().LazyLoad();
        }
    }
}
