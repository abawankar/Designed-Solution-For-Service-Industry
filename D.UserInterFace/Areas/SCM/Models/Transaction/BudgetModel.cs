
using System.Collections.Generic;
using System.Linq;
using DAL.Transaction;
using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using Domain.Interface.Master;
using Domain.Interface.Transaction;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Transaction
{
    public class BudgetModel : Domain.Implementation.Transaction.Budget
    {
        public int NatureId { get; set; }
        public int EmpId { get; set; }
        public int BranchId { get; set; }
    }

    public class BudgetRepository : Repository<BudgetModel>
    {
        public override BudgetModel GetById(int id)
        {
            BudgetDAL dal = new BudgetDAL();
            AutoMapper.Mapper.CreateMap<Budget, BudgetModel>();
            AutoMapper.Mapper.CreateMap<Budget, BudgetModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.Employee.Id));
            BudgetModel model = AutoMapper.Mapper.Map<BudgetModel>(dal.GetById(id));

            return model;
        }

        public override IList<BudgetModel> GetAll()
        {
            BudgetDAL dal = new BudgetDAL();
            AutoMapper.Mapper.CreateMap<Budget, BudgetModel>();
            AutoMapper.Mapper.CreateMap<Budget, BudgetModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.Employee.Id));
            List<BudgetModel> model = AutoMapper.Mapper.Map<List<BudgetModel>>(dal.GetAll().OrderByDescending(x => x.Id));

            return model;
        }

        public override void Edit(BudgetModel obj)
        {
            BudgetDAL dal = new BudgetDAL();
            IBudget bl = dal.GetById(obj.Id);
            bl.Year = obj.Year;
            IEmployee emp = new Employee { Id = obj.EmpId };
            bl.Employee = emp;
            IBusinessNature nat = new BusinessNature { Id = obj.NatureId };
            bl.Nature = nat;
            IBranch branch = new Branch { Id = obj.BranchId };
            bl.Branch = branch;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(BudgetModel obj)
        {
            BudgetDAL dal = new BudgetDAL();
            IBudget bl = new Budget();
            bl.Year = obj.Year;
            IEmployee emp = new Employee { Id = obj.EmpId };
            bl.Employee = emp;
            IBusinessNature nat = new BusinessNature { Id = obj.NatureId };
            bl.Nature = nat;

            IBranch branch = new Branch { Id = obj.BranchId };
            bl.Branch = branch;

            List<IBudgetTrn> trnList = new List<IBudgetTrn>();
            string[] month = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            for (int i = 0; i < 12; i++)
            {
                IBudgetTrn trn = new BudgetTrn();
                trn.BudgetMonth = ((i + 1).ToString() + obj.Year.ToString()).PadLeft(6, '0');
                trn.Month = month[i];
                trn.ContractValue = 0;
                trn.ContractCost = 0;
                trnList.Add(trn);
            }
            bl.BudTrn = trnList;
            dal.InsertOrUpdate(bl);
        }

        public IList<BudgetModel> GetAllByFilter(string year, int branch, int emp, int nature)
        {
            List<BudgetModel> model = new List<BudgetModel>();
            //Year and Branch
            if (!string.IsNullOrEmpty(year) && branch != 0 && emp == 0 && nature == 0)
            {
                model = GetAll().Where(x => x.Year == year && x.Branch.Id == branch).ToList();
            }
            //Year, Branch, Employee
            if (!string.IsNullOrEmpty(year) && branch != 0 && emp != 0 && nature == 0)
            {
                model = GetAll().Where(x => x.Year == year && x.Branch.Id == branch && x.Employee.Id == emp).ToList();
            }
            //Year, Branch, Nature, Employee
            if (!string.IsNullOrEmpty(year) && branch != 0 && emp != 0 && nature != 0)
            {
                model = GetAll().Where(x => x.Year == year && x.Branch.Id == branch && x.Employee.Id == emp && x.Nature.Id == nature).ToList();
            }
            //Year, Branch, Nature
            if (!string.IsNullOrEmpty(year) && branch != 0 && emp == 0 && nature != 0)
            {
                model = GetAll().Where(x => x.Year == year && x.Branch.Id == branch && x.Nature.Id == nature).ToList();
            }
            // emp, nature
            if (string.IsNullOrEmpty(year) && branch == 0 && emp != 0 && nature != 0)
            {
                model = GetAll().Where(x => x.Employee.Id == emp && x.Nature.Id == nature).ToList();
            }
            // year, emp
            if (!string.IsNullOrEmpty(year) && branch == 0 && emp != 0 && nature == 0)
            {
                model = GetAll().Where(x => x.Year == year && x.Employee.Id == emp).ToList();
            }
            // Branch, emp
            if (string.IsNullOrEmpty(year) && branch != 0 && emp != 0 && nature == 0)
            {
                model = GetAll().Where(x => x.Branch.Id == branch && x.Employee.Id == emp).ToList();
            }
            return model;
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public void EditBudgetTrn(BudgetTrnModel obj)
        {
            BudgetTrnDAL dal = new BudgetTrnDAL();
            IBudgetTrn bl = dal.GetById(obj.Id);
            bl.ContractValue = obj.ContractValue;
            bl.ContractCost = obj.ContractCost;
            dal.InsertOrUpdate(bl);
        }
    }
}