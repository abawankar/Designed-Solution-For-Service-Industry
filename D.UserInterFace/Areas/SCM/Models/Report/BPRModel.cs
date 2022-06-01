using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Models.Masters;
using System.Collections.Generic;
using System.Linq;

namespace D.UserInterFace.Areas.SCM.Models.Report
{
    public class BPRModel
    {
        public IList<BudgetModel> Budget { get; set; }
        public IList<NBOModel> NBO { get; set; }
        public IList<MonthModel> month { get; set; }
    }

    public class MonthModel
    {
        public string Month { get; set; }
    }

    public class BPRRepository
    {
        public IList<BudgetModel> GetAllBudget()
        {
            BudgetRepository dal = new BudgetRepository();
            IList<BudgetModel> list = dal.GetAll();
            return list;
        }

        public IList<NBOModel> GetAllNBO()
        {
            NBORepository dal = new NBORepository();
            IList<NBOModel> list = dal.GetAll();
            return list;
        }

        public IList<MonthModel> GetMonth(string year)
        {
            IList<MonthModel> bl = new List<MonthModel>();
            //dt.Columns.Add(new DataColumn("Month", typeof(string)));
            for (int i = 1; i < 13; i++)
            {
                MonthModel m = new MonthModel();
                m.Month = i.ToString().PadLeft(2, '0') + year;
                bl.Add(m);
            }
            return bl;
        }

        public BPRModel GetBy(string year, int branch)
        {
            try
            {
                BPRModel model = new BPRModel();
                model.Budget = GetAllBudget().Where(x => x.Year == year && x.Branch.Id == branch).ToList();
                model.NBO = GetAllNBO().Where(x => x.EventMonth.Contains(year) && x.Branch.Id == branch).ToList();
                model.month = GetMonth(year);
                return model;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public BPRModel GetByEmployee(string year, int empid)
        {
            try
            {
                BPRModel model = new BPRModel();
                model.Budget = GetAllBudget().Where(x => x.Year == year && x.EmpId == empid).ToList();
                model.NBO = GetAllNBO().Where(x => x.EventMonth.Contains(year) && x.EmpId == empid).ToList();
                model.month = GetMonth(year);
                return model;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public BPRModel GetByNature(string year, int natid, int branch)
        {
            try
            {
                BPRModel model = new BPRModel();
                model.Budget = GetAllBudget().Where(x => x.Year == year && x.NatureId == natid && x.Branch.Id == branch).ToList();
                if (natid == 6)
                {
                    model.NBO = GetAllNBO().Where(x => x.EventMonth.Contains(year) && (x.NatureId == 1 || x.NatureId == 2) && x.Branch.Id == branch).ToList();
                }
                else
                {
                    model.NBO = GetAllNBO().Where(x => x.EventMonth.Contains(year) && x.NatureId == natid && x.Branch.Id == branch).ToList();
                }

                model.month = GetMonth(year);
                return model;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public BPRModel GetBYManager(string year, int natid, string name)
        {
            try
            {
                BPRModel model = new BPRModel();
                EmployeeRepository dal = new EmployeeRepository();
                int deptId = dal.GetManagerDept(name.ToUpper());
                var data = dal.GetAll().Where(x => x.Department.Id == deptId).Select(x => x.Id).ToList();
                model.Budget = GetAllBudget().Where(x => data.Contains(x.EmpId)).ToList();
                model.NBO = GetAllNBO().Where(x => data.Contains(x.EmpId)).ToList();

                model.Budget = model.Budget.Where(x => x.Year == year && x.NatureId == natid).ToList();
                if (natid == 6)
                {
                    model.NBO = model.NBO.Where(x => x.EventMonth.Contains(year) && (x.NatureId == 1 || x.NatureId == 2)).ToList();
                }
                else
                {
                    model.NBO = model.NBO.Where(x => x.EventMonth.Contains(year) && x.NatureId == natid).ToList();
                }

                model.month = GetMonth(year);

                return model;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public BPRModel GetBYManager(string year, string name)
        {
            try
            {
                BPRModel model = new BPRModel();
                EmployeeRepository dal = new EmployeeRepository();
                int deptid = dal.GetManagerDept(name.ToUpper());
                var data = dal.GetAll().Where(x => x.Department.Id == deptid).Select(x => x.Id).ToList();
                model.Budget = GetAllBudget().Where(x => data.Contains(x.EmpId) && x.Year == year).ToList();
                model.NBO = GetAllNBO().Where(x => data.Contains(x.EmpId) && x.EventMonth.Contains(year)).ToList();
                model.month = GetMonth(year);
                return model;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public BPRModel GetBy(string year, int empid, int natid)
        {
            try
            {
                BPRModel model = new BPRModel();
                model.Budget = GetAllBudget().Where(x => x.Year == year && x.EmpId == empid && x.NatureId == natid).ToList();

                if (natid == 6)
                {
                    model.NBO = GetAllNBO().Where(x => x.EventMonth.Contains(year) && x.EmpId == empid && (x.NatureId == 1 || x.NatureId == 2)).ToList();
                }
                else
                {
                    model.NBO = GetAllNBO().Where(x => x.EventMonth.Contains(year) && x.EmpId == empid && x.NatureId == natid).ToList();
                }

                model.month = GetMonth(year);
                return model;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}