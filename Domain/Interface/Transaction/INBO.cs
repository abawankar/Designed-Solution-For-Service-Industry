using System;
using Domain.Interface.Master;

namespace Domain.Interface.Transaction
{
    public interface INBO
    {
        int Id { get; set; }
        IBusinessNature Nature { get; set; }
        string RequestMonth { get; set; }
        DateTime RequestDate { get; set; }
        string FileNumber { get; set; }
        IEmployee FileHandler { get; set; }
        IClient ClientName { get; set; }
        ICountry ClientCountry { get; set; }
        string ContactName { get; set; }
        string EmailId { get; set; }
        string PhoneNo { get; set; }
        string MobileNo { get; set; }
        string Fax { get; set; }
        string EventName { get; set; }
        int PaxNo { get; set; }
        DateTime? CheckinDate { get; set; }
        DateTime? CheckOutDate { get; set; }
        string EventMonth { get; set; }
        IEnquirySource EnquirySource { get; set; }
        IEnquiryStatus Status { get; set; }
        DateTime? StatusDate { get; set; }
        double? ContractValue { get; set; }
        double? ContractCost { get; set; }
        double? Margin { get; }
        double? MarginP { get; }
        string Remarks { get; set; }
        IBranch Branch { get; set; }
        int ContactId { get; set; }

    }
}
