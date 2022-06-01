
namespace Domain.Interface.Master
{
    public interface IClient
    {
        int Id { get; set; }
        string Name { get; set; }
        ICountry Country { get; set; }
        string ClientGroup { get; set; }
        string Remarks { get; set; }
    }
}
