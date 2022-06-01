using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class Department : IDepartment
    {
        public virtual int Id { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
    }
}
