using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class Group : IGroup
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
;