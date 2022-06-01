using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class BusinessNature : IBusinessNature
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
