using System;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class LeastProducerModel
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public int ActId { get; set; }
        public int AccountId { get; set; }
        public double Days { get; set; }
        public Double Business { get; set; }
        public DateTime Date { get; set; }
        public string Owner { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Branch { get; set; }
    }
}