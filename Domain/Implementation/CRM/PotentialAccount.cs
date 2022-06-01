using System.Collections.Generic;
using Domain.Interface.CRM;
using Domain.Interface.Master;

namespace Domain.Implementation.CRM
{
    public class PotentialAccount : IPotentialAccount
    {
        public virtual int Id { get; set; }
        public virtual string AccountName { get; set; }
        public virtual int AccountId { get; set; }
        public virtual string Type { get; set; }
        public virtual string Website { get; set; }
        public virtual string Description { get; set; }
        public virtual string Phone { get; set; }
        public virtual IIndustry Industry { get; set; }
        public virtual ICountry Country { get; set; }
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string ZipPostalCode { get; set; }
        public virtual IList<IPotentialContact> Contact { get; set; }
    }
}
