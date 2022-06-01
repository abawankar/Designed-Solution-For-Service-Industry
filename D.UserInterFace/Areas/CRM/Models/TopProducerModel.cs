
namespace D.UserInterFace.Areas.CRM.Models
{
    public class TopProducerModel
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
        public double ContractValue { get; set; }
        public double ContractCost { get; set; }
        public double Margin { get; set; }
        public double MarginP { get; set; }
        public int TotalFiles { get; set; }
        public int TotalPax { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Branch { get; set; }
    }
}