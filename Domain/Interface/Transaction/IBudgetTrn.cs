
namespace Domain.Interface.Transaction
{
    public interface IBudgetTrn
    {
        int Id { get; set; }
        string BudgetMonth { get; set; }
        string Month { get; set; }
        double? ContractValue { get; set; }
        double? ContractCost { get; set; }
        double? Margin { get; }
        double? MarginP { get; }
    }
}
