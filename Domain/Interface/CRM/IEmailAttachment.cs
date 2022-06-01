
namespace Domain.Interface.CRM
{
    public interface IEmailAttachment
    {
        int Id { get; set; }
        string Filetype { get; set; }
        string FileName { get; set; }
        string FileOnServer { get; set; }
        string FileSize { get; set; }
        int AttachType { get; set; }
        int Width { get; set; }
        int Height { get; set; }

    }
}
