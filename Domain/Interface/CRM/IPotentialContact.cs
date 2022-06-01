using Domain.Interface.Master;

namespace Domain.Interface.CRM
{
    public interface IPotentialContact
    {
        int Id { get; set; }
        string Salutation { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
        string Title { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string PhoneDirect { get; set; }
        string Mobile { get; set; }
        string Fax { get; set; }
        string Messanger { get; set; }
        bool EmailOpt { get; set; }
        string Department { get; set; }
        ICountry Country { get; set; }
        string Street { get; set; }
        string City { get; set; }
        string StateProvince { get; set; }
        string ZipPostalCode { get; set; }
        string AccountName { get; set; }
    }
}
