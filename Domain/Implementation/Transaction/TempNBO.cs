using System;
using Domain.Interface.Master;
using Domain.Interface.Transaction;

namespace Domain.Implementation.Transaction
{
    public class TempNBO : ITempNBO
    {
        public virtual int Id { get; set; }
        public virtual int NBOId { get; set; }
        public virtual IBusinessNature Nature { get; set; }
        public virtual string RequestMonth { get; set; }
        public virtual DateTime RequestDate { get; set; }
        public virtual string FileNumber { get; set; }
        public virtual IEmployee FileHandler { get; set; }
        public virtual IClient ClientName { get; set; }
        public virtual ICountry ClientCountry { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string EmailId { get; set; }
        public virtual string PhoneNo { get; set; }
        public virtual string MobileNo { get; set; }
        public virtual string Fax { get; set; }
        public virtual string EventName { get; set; }
        public virtual int PaxNo { get; set; }
        public virtual DateTime? CheckinDate { get; set; }
        public virtual DateTime? CheckOutDate { get; set; }
        public virtual string EventMonth { get; set; }
        public virtual IEnquirySource EnquirySource { get; set; }
        public virtual IEnquiryStatus Status { get; set; }
        public virtual DateTime? StatusDate { get; set; }
        public virtual double? ContractValue { get; set; }
        public virtual double? ContractCost { get; set; }
        public virtual double? Margin { get { return ContractValue - ContractCost; } }
        public virtual double? MarginP
        {

            get
            {
                if (Margin != 0 && ContractValue != 0) { return (Margin / ContractValue) * 100; }
                return 0;
            }
        }
        public virtual string Remarks { get; set; }
        public virtual IBranch Branch { get; set; }
    }
}
