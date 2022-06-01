using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Interface.Transaction
{
    public interface IBudget
    {
        int Id { get; set; }
        string Year { get; set; }
        IEmployee Employee { get; set; }
        IBusinessNature Nature { get; set; }
        IList<IBudgetTrn> BudTrn { get; set; }
        IBranch Branch { get; set; }
    }
}
