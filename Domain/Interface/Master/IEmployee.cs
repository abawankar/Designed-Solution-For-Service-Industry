
using System.Collections.Generic;
namespace Domain.Interface.Master
{
    public interface IEmployee
    {
        int Id { get; set; }
        string EmpCode { get; set; }
        string EmpName { get; set; }
        string AppLogin { get; set; }
        string MailId { get; set; }
        string Role { get; set; }
        double? CostPerHour { get; set; }
        bool Active { get; set; }
        ICompany Company { get; set; }
        IBranch Branch { get; set; }
        IDepartment Department { get; set; }
        IGroup Group { get; set; }
        IList<IEmpRights> EmpRights { get; set; }
    }
}
