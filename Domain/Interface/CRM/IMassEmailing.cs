using System;
using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Interface.CRM
{
    public interface IMassEmailing
    {
        int Id { get; set; }
        string Name { get; set; }
        bool Bcc { get; set; }
        string ReplyTo { get; set; }
        IList<IContact> ContactGroup { get; set; }
        IEmailTemplate EmailTemplate { get; set; }
        IEmployee Owner { get; set; }
        int Status { get; set; }
        DateTime? Schedule { get; set; }
        DateTime? ServerTime { get; set; }
        string TimeZone { get; set; }
        string Salutation { get; set; }

    }
}
