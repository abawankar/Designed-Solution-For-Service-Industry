
using System.Collections.Generic;
using Domain.Interface.Master;
namespace Domain.Interface.CRM
{
    public interface IPotentialAccount
    {
        int Id { get; set; }
        string AccountName { get; set; }
        int AccountId { get; set; }
        string Type { get; set; }
        string Website { get; set; }
        string Description { get; set; }
        string Phone { get; set; }
        IIndustry Industry { get; set; }
        ICountry Country { get; set; }
        string Street { get; set; }
        string City { get; set; }
        string State { get; set; }
        string ZipPostalCode { get; set; }
        IList<IPotentialContact> Contact { get; set; }
    }
}
