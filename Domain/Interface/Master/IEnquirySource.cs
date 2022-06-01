
using System;
namespace Domain.Interface.Master
{
    public interface IEnquirySource
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime? AppointmentDate { get; set; }
        DateTime? TerminationDate { get; set; }
        string RetainerFee { get; set; }
        string CommLeisure { get; set; }
        string CommMice { get; set; }
        bool Active { get; set; }
    }
}
