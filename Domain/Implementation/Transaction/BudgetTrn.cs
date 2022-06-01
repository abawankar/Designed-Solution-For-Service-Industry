using Domain.Interface.Transaction;

namespace Domain.Implementation.Transaction
{
    public class BudgetTrn : IBudgetTrn
    {
        public virtual int Id { get; set; }
        public virtual string BudgetMonth { get; set; }
        public virtual string Month { get; set; }
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
    }
}
