using System;
using System.Collections.Generic;
using Domain.Interface.CRM;
using Domain.Interface.Master;

namespace Domain.Implementation.CRM
{
    public class ContactGroup : IContactGroup
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IEmployee GroupOwner { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual string Note { get; set; }
        public virtual IList<IContact> ContactList { get; set; }
    }
}
