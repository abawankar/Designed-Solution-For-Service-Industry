using Domain.Interface.CRM;

namespace Domain.Implementation.CRM
{
    public class EmailAttachment : IEmailAttachment
    {
        public virtual int Id { get; set; }
        public virtual string Filetype { get; set; }
        public virtual string FileName { get; set; }
        public virtual string FileOnServer { get; set; }
        public virtual string FileSize { get; set; }
        public virtual int AttachType { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
    }
}
