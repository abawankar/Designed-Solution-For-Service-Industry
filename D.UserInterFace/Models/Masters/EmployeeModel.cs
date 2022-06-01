
using System.Collections.Generic;
using System.Linq;
using DAL.Master;
using DAL.Transaction;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using Domain.Interface.Transaction;

namespace D.UserInterFace.Models.Masters
{
    public class EmployeeModel : Domain.Implementation.Master.Employee
    {
        public int CompId { get; set; }
        public int BranchId { get; set; }
        public int DeptId { get; set; }
        public int GroupId { get; set; }
    }

    public class EmployeeRepository : Repository<EmployeeModel>
    {
        //Employee abstract class implementation
        #region --- Employee abstract class

        public override EmployeeModel GetById(int id)
        {
            EmployeeDAL dal = new EmployeeDAL();
            AutoMapper.Mapper.CreateMap<Employee, EmployeeModel>();
            AutoMapper.Mapper.CreateMap<Employee, EmployeeModel>()
                .ForMember(dest => dest.CompId, opt => opt.MapFrom(scr => scr.Company.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(scr => scr.Group.Id))
                .ForMember(dest => dest.DeptId, opt => opt.MapFrom(scr => scr.Department.Id));
            EmployeeModel model = AutoMapper.Mapper.Map<EmployeeModel>(dal.GetById(id));

            return model;
        }

        public override IList<EmployeeModel> GetAll()
        {
            EmployeeDAL dal = new EmployeeDAL();
            AutoMapper.Mapper.CreateMap<Employee, EmployeeModel>();
            AutoMapper.Mapper.CreateMap<Employee, EmployeeModel>()
                .ForMember(dest => dest.CompId, opt => opt.MapFrom(scr => scr.Company.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(scr => scr.Group.Id))
                .ForMember(dest => dest.DeptId, opt => opt.MapFrom(scr => scr.Department.Id));
            List<EmployeeModel> model = AutoMapper.Mapper.Map<List<EmployeeModel>>(dal.GetAll());

            return model;
        }

        public IList<EmployeeModel> GetEmpById(int id)
        {
            EmployeeDAL dal = new EmployeeDAL();
            AutoMapper.Mapper.CreateMap<Employee, EmployeeModel>();
            AutoMapper.Mapper.CreateMap<Employee, EmployeeModel>()
                .ForMember(dest => dest.CompId, opt => opt.MapFrom(scr => scr.Company.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(scr => scr.Group.Id))
                .ForMember(dest => dest.DeptId, opt => opt.MapFrom(scr => scr.Department.Id));
            List<EmployeeModel> model = AutoMapper.Mapper.Map<List<EmployeeModel>>(dal.GetEmpById(id));

            return model.ToList();
        }

        public IList<EmployeeModel> SelectEmployee()
        {
            List<EmployeeModel> model = new List<EmployeeModel>();
            model.Add(new EmployeeModel { Id = 0, EmpName = "* Select *" });

            return model;
        }

        public override void Edit(EmployeeModel obj)
        {
            EmployeeDAL dal = new EmployeeDAL();
            NBODAL nbo = new NBODAL();
            List<INBO> nbobl = nbo.GetAll().Where(x => x.FileHandler.AppLogin.Contains(obj.AppLogin.ToUpper())).ToList();
            IEmployee bl = dal.GetById(obj.Id);
            bl.CostPerHour = obj.CostPerHour;
            if (nbobl.Count == 0)
            {
                bl.EmpCode = obj.EmpCode;
                bl.EmpName = obj.EmpName;
                bl.AppLogin = obj.AppLogin;
                bl.MailId = obj.MailId;
            }
            ICompany comp = new Company { Id = obj.CompId };
            bl.Company = comp;

            IBranch bran = new Branch { Id = obj.BranchId };
            bl.Branch = bran;

            IDepartment dept = new Department { Id = obj.DeptId };
            bl.Department = dept;

            IGroup group = new Group { Id = obj.GroupId };
            bl.Group = group;

            dal.InsertOrUpdate(bl);
        }

        public override void Insert(EmployeeModel obj)
        {
            EmployeeDAL dal = new EmployeeDAL();
            IEmployee bl = new Employee();
            bl.EmpCode = obj.EmpCode;
            bl.EmpName = obj.EmpName;
            bl.AppLogin = obj.AppLogin;
            bl.MailId = obj.MailId;
            bl.CostPerHour = obj.CostPerHour;
            ICompany comp = new Company { Id = obj.CompId };
            bl.Company = comp;

            IBranch bran = new Branch { Id = obj.BranchId };
            bl.Branch = bran;

            IDepartment dept = new Department { Id = obj.DeptId };
            bl.Department = dept;

            IGroup group = new Group { Id = obj.GroupId };
            bl.Group = group;

            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public int GetManagerDept(string applogin)
        {
            EmployeeDAL dal = new EmployeeDAL();
            AutoMapper.Mapper.CreateMap<Department, DepartmentModel>();
            DepartmentModel model = AutoMapper.Mapper.Map<DepartmentModel>(dal.GetByAppLogin(applogin).Department);

            return model.Id;
        }

        public int GetByName(string applogin)
        {
            EmployeeDAL dal = new EmployeeDAL();
            IEmployee bl = dal.GetByAppLogin(applogin);
            int empId = bl == null ? 0 : bl.Id;
            return empId;
        }

        public IEmployee GetBy(string applogin)
        {
            EmployeeDAL dal = new EmployeeDAL();
            return dal.GetByAppLogin(applogin); ;
        }

        public int GetBranchId(string applogin)
        {
            EmployeeDAL dal = new EmployeeDAL();
            return dal.GetByAppLogin(applogin).Branch.Id;
        }

        #endregion

        public static string EmployeeName(int id)
        {
            if (id == 0)
            {
                return "All";
            }
            else
            {
                EmployeeRepository e = new EmployeeRepository();
                return e.GetById(id).EmpName;
            }
        }
        
        public static string ManagerMailId(int deptid)
        {
            EmployeeRepository dal = new EmployeeRepository();
            IEmployee emp = dal.GetAll().Where(x => x.Department.Id == deptid && x.Role == "Manager").SingleOrDefault();
            return emp.MailId;
        }

        public static void AddEmpRights(int id, string rightList)
        {
            EmployeeDAL dal = new EmployeeDAL();
            dal.AddRights(id, rightList);
        }

        public static void DeleteRights(int empId, int RightId)
        {
            EmployeeDAL dal = new EmployeeDAL();
            IEmployee bl = dal.GetById(empId);

            EmpRightsDAL cdal = new EmpRightsDAL();
            IEmpRights rights = cdal.GetById(RightId);

            int i = bl.EmpRights.IndexOf(rights);
            bl.EmpRights.RemoveAt(i);
            dal.InsertOrUpdate(bl);
        }
    }
}