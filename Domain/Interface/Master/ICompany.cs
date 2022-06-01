
using System.Collections.Generic;
namespace Domain.Interface.Master
{
    public interface ICompany
    {
        int Id { get; set; }
        string Code { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        string PhoneNo { get; set; }
        string FaxNo { get; set; }
        IList<IBranch> Branches { get; set; }
    }
}
