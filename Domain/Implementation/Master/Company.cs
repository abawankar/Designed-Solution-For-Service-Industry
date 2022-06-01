using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class Company : ICompany
    {
        public virtual int Id { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual string PhoneNo { get; set; }
        public virtual string FaxNo { get; set; }
        public virtual IList<IBranch> Branches { get; set; }
    }
}
