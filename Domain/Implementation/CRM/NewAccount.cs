using System;
using System.Collections.Generic;
using Domain.Interface;
using Domain.Interface.CRM;
using Domain.Interface.Master;

namespace Domain.Implementation.CRM
{
    public class NewAccount : INewAccount
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
        public virtual string ZipPostalCode { get; set; }
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Email { get; set; }
        public virtual IList<IContact> Contact { get; set; }
        public virtual IEmployee Owner { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual IList<IAccountType> ActType { get; set; }
        public virtual string ModifyDate { get; set; }
        public virtual IEmployee Modifyby { get; set; }
    }
}
