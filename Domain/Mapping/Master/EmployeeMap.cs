using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public sealed class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.EmpCode);
            Map(x => x.EmpName);
            Map(x => x.AppLogin);
            Map(x => x.MailId);
            Map(x => x.Role);
            Map(x => x.CostPerHour);
            Map(x => x.Active);
            References<Company>(x => x.Company).Column("CompId").ForeignKey("fk_emp_compid").LazyLoad();
            References<Branch>(x => x.Branch).Column("BranchId").ForeignKey("fk_emp_branchid").LazyLoad();
            References<Department>(x => x.Department).Column("DeptId").ForeignKey("fk_emp_deptid").LazyLoad();
            References<Group>(x => x.Group).Column("GroupId").ForeignKey("fk_emp_groupid").LazyLoad();
            HasManyToMany<EmpRights>(x => x.EmpRights).ParentKeyColumn("EmpId").ChildKeyColumn("RightsId").LazyLoad();
        }
    }
}
