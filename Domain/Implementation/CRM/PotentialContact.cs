using Domain.Interface.CRM;
using Domain.Interface.Master;

namespace Domain.Implementation.CRM
{
    public class PotentialContact : IPotentialContact
    {
        public virtual int Id { get; set; }
        public virtual string Salutation { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string PhoneDirect { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Messanger { get; set; }
        public virtual bool EmailOpt { get; set; }
        public virtual string Department { get; set; }
        public virtual ICountry Country { get; set; }
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string StateProvince { get; set; }
        public virtual string ZipPostalCode { get; set; }
        public virtual string AccountName { get; set; }
    }
}
