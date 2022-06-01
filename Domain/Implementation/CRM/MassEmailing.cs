using System;
using System.Collections.Generic;
using Domain.Interface.CRM;
using Domain.Interface.Master;

namespace Domain.Implementation.CRM
{
    public class MassEmailing : IMassEmailing
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual bool Bcc { get; set; }
        public virtual string ReplyTo { get; set; }
        public virtual IList<IContact> ContactGroup { get; set; }
        public virtual IEmailTemplate EmailTemplate { get; set; }
        public virtual int Status { get; set; }
        public virtual DateTime? Schedule { get; set; }
        public virtual DateTime? ServerTime { get; set; }
        public virtual string TimeZone { get; set; }
        public virtual IEmployee Owner { get; set; }
        public virtual string Salutation { get; set; }

    }
}
