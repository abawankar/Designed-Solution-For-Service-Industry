using System;
using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Interface.CRM
{
    public interface IContactGroup
    {
        int Id { get; set; }
        string Name { get; set; }
        IEmployee GroupOwner { get; set; }
        DateTime? Date { get; set; }
        string Note { get; set; }
        IList<IContact> ContactList { get; set; }
    }
}
