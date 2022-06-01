using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class Supplier : ISupplier
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ISupplierCategory Category { get; set; }
        public virtual string AVLCode { get; set; }
    }
}
