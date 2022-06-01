using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class ContactType : IContactType
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
