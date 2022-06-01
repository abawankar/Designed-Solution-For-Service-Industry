using System;
using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Interface.CRM
{
    public interface IAccountMailLog
    {
        int Id { get; set; }
        INewAccount AccountId { get; set; }
        IList<IContact> ContactList { get; set; }
        string CC { get; set; }
        string Subject { get; set; }
        string Mailbody { get; set; }
        IEmployee Owner { get; set; }
        DateTime Date { get; set; }

    }
}
