using System;
using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class EnquirySource : IEnquirySource
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? AppointmentDate { get; set; }
        public virtual DateTime? TerminationDate { get; set; }
        public virtual string RetainerFee { get; set; }
        public virtual string CommLeisure { get; set; }
        public virtual string CommMice { get; set; }
        public virtual bool Active { get; set; }
    }
}
