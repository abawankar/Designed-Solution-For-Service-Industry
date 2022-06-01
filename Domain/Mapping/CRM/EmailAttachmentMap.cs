using Domain.Implementation.CRM;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public class EmailAttachmentMap : ClassMap<EmailAttachment>
    {
        public EmailAttachmentMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Filetype);
            Map(x => x.FileName);
            Map(x => x.FileOnServer);
            Map(x => x.FileSize);
            Map(x => x.AttachType);
            Map(x => x.Width);
            Map(x => x.Height);
        }
    }
}
