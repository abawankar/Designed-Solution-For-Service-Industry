using System;
using Domain.Interface.CRM;

namespace Domain.Implementation.CRM
{
    public class OneStayInTouch : IOneStayInTouch
    {
        public virtual int Id { get; set; }
        public virtual IContact Contact { get; set; }
        public virtual string CC { get; set; }
        public virtual string BCC { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Notes { get; set; }
        public virtual string Signature { get; set; }
        public virtual DateTime Date { get; set; }
    }
}
