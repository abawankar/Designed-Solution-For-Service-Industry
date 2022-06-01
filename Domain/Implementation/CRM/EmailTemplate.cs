using System.Collections.Generic;
using Domain.Interface.CRM;

namespace Domain.Implementation.CRM
{
    public class EmailTemplate : IEmailTemplate
    {
        public virtual int Id { get; set; }
        public virtual string TemplateName { get; set; }
        public virtual string UniqueName { get; set; }
        public virtual string Encoding { get; set; }
        public virtual string Description { get; set; }
        public virtual string Subject { get; set; }
        public virtual string EmailBody { get; set; }
        public virtual int Type { get; set; }

        public virtual IList<IEmailAttachment> Attachment { get; set; }
    }
}
