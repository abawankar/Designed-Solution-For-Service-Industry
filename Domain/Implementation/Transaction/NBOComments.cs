using System;
using Domain.Interface.Transaction;

namespace Domain.Implementation.Transaction
{
    public class NBOComments : INBOComments
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Comments { get; set; }
        public virtual INBO NBO { get; set; }
    }
}
