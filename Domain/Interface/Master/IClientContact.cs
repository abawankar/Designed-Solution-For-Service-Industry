
namespace Domain.Interface.Master
{
    public interface IClientContact
    {
        int Id { get; set; }
        string ContactName { get; set; }
        string EmailId { get; set; }
        string Phone { get; set; }
        string Mobile { get; set; }
        string Fax { get; set; }
    }
}
