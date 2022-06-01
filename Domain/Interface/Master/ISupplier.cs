
namespace Domain.Interface.Master
{
    public interface ISupplier
    {
        int Id { get; set; }
        string Name { get; set; }
        ISupplierCategory Category { get; set; }
        string AVLCode { get; set; }
    }
}
