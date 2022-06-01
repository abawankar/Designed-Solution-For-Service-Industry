using Domain.Interface;

namespace Domain.Implementation
{
    public class AccountType : IAccountType
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
