using System;
using System.Collections.Generic;
using Domain.Interface.CRM;
using Domain.Interface.Master;

namespace Domain.Implementation.CRM
{
    public class AccountMailLog : IAccountMailLog
    {
        public virtual int Id { get; set; }
        public virtual INewAccount AccountId { get; set; }
        public virtual IList<IContact> ContactList { get; set; }
        public virtual string CC { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Mailbody { get; set; }
        public virtual IEmployee Owner { get; set; }
        public virtual DateTime Date { get; set; }
    }
}
