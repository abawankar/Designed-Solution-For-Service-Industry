
using System.Collections.Generic;
namespace Domain.Interface.CRM
{
    public interface IEmailTemplate
    {
        int Id { get; set; }
        string TemplateName { get; set; }
        string UniqueName { get; set; }
        string Encoding { get; set; }
        string Description { get; set; }
        string Subject { get; set; }
        string EmailBody { get; set; }
        int Type { get; set; }

        IList<IEmailAttachment> Attachment { get; set; }
    }
}
