using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class Employee : IEmployee
    {
        public virtual int Id { get; set; }
        public virtual string EmpCode { get; set; }
        public virtual string EmpName { get; set; }
        public virtual string AppLogin { get; set; }
        public virtual string MailId { get; set; }
        public virtual string Role { get; set; }
        public virtual double? CostPerHour { get; set; }
        public virtual bool Active { get; set; }
        public virtual ICompany Company { get; set; }
        public virtual IBranch Branch { get; set; }
        public virtual IDepartment Department { get; set; }
        public virtual IGroup Group { get; set; }
        public virtual IList<IEmpRights> EmpRights { get; set; }
    }
}
