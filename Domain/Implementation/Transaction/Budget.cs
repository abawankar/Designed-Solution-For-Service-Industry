using System.Collections.Generic;
using Domain.Interface.Master;
using Domain.Interface.Transaction;

namespace Domain.Implementation.Transaction
{
    public class Budget : IBudget
    {
        public virtual int Id { get; set; }
        public virtual string Year { get; set; }
        public virtual IEmployee Employee { get; set; }
        public virtual IBusinessNature Nature { get; set; }
        public virtual IList<IBudgetTrn> BudTrn { get; set; }
        public virtual IBranch Branch { get; set; }
    }
}
