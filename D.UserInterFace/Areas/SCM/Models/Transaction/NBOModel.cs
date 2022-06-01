using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Transaction;
using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using Domain.Interface.Master;
using Domain.Interface.Transaction;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.SCM.Models.Transaction
{
    public class NBOModel : Domain.Implementation.Transaction.NBO
    {
        public int NatureId { get; set; }
        public int EmpId { get; set; }
        public int ClientId { get; set; }
        public int CountryId { get; set; }
        public int EnqSourceId { get; set; }
        public int StatusId { get; set; }
        public int BranchId { get; set; }
        public int linkContact { get; set; }
    }

    public class NBORepository : Repository<NBOModel>
    {
        public override NBOModel GetById(int id)
        {
            NBODAL dal = new NBODAL();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            NBOModel model = AutoMapper.Mapper.Map<NBOModel>(dal.GetById(id));

            return model;
        }

        public IList<NBOModel> GetByFile(string file)
        {
            NBODAL dal = new NBODAL();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            IList<NBOModel> model = AutoMapper.Mapper.Map<List<NBOModel>>(dal.GetByFile(file));

            return model;
        }

        public override IList<NBOModel> GetAll()
        {
            NBODAL dal = new NBODAL();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            List<NBOModel> model = AutoMapper.Mapper.Map<List<NBOModel>>(dal.GetAll());

            return model;
        }

        public IList<NBOModel> GetTop()
        {
            NBODAL dal = new NBODAL();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            List<NBOModel> model = AutoMapper.Mapper.Map<List<NBOModel>>(dal.GetTop());

            return model;
        }

        public IList<NBOModel> GetByEmployee(int empid)
        {
            NBODAL dal = new NBODAL();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            List<NBOModel> model = AutoMapper.Mapper.Map<List<NBOModel>>(dal.GetbyEmployee(empid));

            return model;
        }

        public IList<NBOModel> GetConfirmedFiles()
        {
            NBODAL dal = new NBODAL();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>();
            AutoMapper.Mapper.CreateMap<NBO, NBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            List<NBOModel> model = AutoMapper.Mapper.Map<List<NBOModel>>(dal.GetConfirmFile());

            return model;
        }

        public IList<NBOModel> GetBy(int empid)
        {
            //List<NBOModel> model = GetAll().Where(x => x.FileHandler.Id == empid).ToList();
            List<NBOModel> model = GetByEmployee(empid).ToList();
            return model;
        }

        public IList<NBOModel> GetByNature(int natid)
        {
            List<NBOModel> model = new List<NBOModel>();
            model = GetAll().Where(x => x.NatureId == natid).ToList();
            return model;
        }

        public IList<NBOModel> GetByManager(string name)
        {
            EmployeeRepository dal = new EmployeeRepository();
            int deptid = dal.GetManagerDept(name.ToUpper());
            var data = dal.GetAll().Where(x => x.Department.Id == deptid).Select(x => x.Id).ToList();

            List<NBOModel> model = GetAll().Where(x => data.Contains(x.EmpId)).ToList();
            return model;
        }

        public IList<NBOModel> GetByManager(string name, int natid)
        {
            EmployeeRepository dal = new EmployeeRepository();
            int deptid = dal.GetManagerDept(name.ToUpper());
            var data = dal.GetAll().Where(x => x.Department.Id == deptid).Select(x => x.Id).ToList();

            List<NBOModel> model = new List<NBOModel>();
            model = GetAll().Where(x => data.Contains(x.EmpId) && x.NatureId == natid).ToList();
            return model;
        }

        public IList<NBOModel> GetBy(int empid, int natid)
        {
            List<NBOModel> model = new List<NBOModel>();
            model = GetByEmployee(empid).Where(x => x.NatureId == natid).ToList();
            return model;
        }

        public IList<NBOModel> GetAllByFilter(List<NBOModel> model, string status, int branch, int emp, int nature, int source)
        {
            //Branch
            if (string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature == 0)
            {
                model = model.Where(x => x.Branch.Id == branch).ToList();
            }
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature == 0)
            {
                string[] id = status.Split(',');
                model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch).ToList();
            }
            //status, Branch, Employee
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp != 0 && nature == 0)
            {
                string[] id = status.Split(',');
                model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.FileHandler.Id == emp).ToList();
            }
            //status, Branch, Nature, Employee
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp != 0 && nature != 0)
            {
                string[] id = status.Split(',');
                if (nature == 6)
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.FileHandler.Id == emp && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.FileHandler.Id == emp && x.Nature.Id == nature).ToList();
                }
            }
            //status, Branch, Nature
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature != 0)
            {
                string[] id = status.Split(',');
                if (nature == 6)
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.Nature.Id == nature).ToList();
                }
            }
            //Branch, Nature
            if (string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature != 0)
            {
                if (nature == 6)
                {
                    model = model.Where(x => x.Branch.Id == branch && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => x.Branch.Id == branch && x.Nature.Id == nature).ToList();
                }
            }
            // emp, nature
            if (string.IsNullOrEmpty(status) && branch == 0 && emp != 0 && nature != 0)
            {
                if (nature == 6)
                {
                    model = model.Where(x => x.FileHandler.Id == emp && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => x.FileHandler.Id == emp && x.Nature.Id == nature).ToList();
                }
            }
            // Status, emp
            if (!string.IsNullOrEmpty(status) && branch == 0 && emp != 0 && nature == 0)
            {
                string[] id = status.Split(',');
                model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.FileHandler.Id == emp).ToList();
            }
            // Branch, emp
            if (string.IsNullOrEmpty(status) && branch != 0 && emp != 0 && nature == 0)
            {
                model = model.Where(x => x.Branch.Id == branch && x.FileHandler.Id == emp).ToList();
            }
            // nature, status
            if (!string.IsNullOrEmpty(status) && branch == 0 && emp == 0 && nature != 0)
            {
                string[] id = status.Split(',');
                if (nature == 6)
                {
                    model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2) && id.Contains(x.Status.Id.ToString())).ToList();
                }
                else
                {
                    model = model.Where(x => x.Nature.Id == nature && id.Contains(x.Status.Id.ToString())).ToList();
                }
            }
            // nature
            if (string.IsNullOrEmpty(status) && branch == 0 && emp == 0 && nature != 0)
            {
                if (nature == 6)
                {
                    model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => x.Nature.Id == nature).ToList();
                }
            }
            // employee
            if (string.IsNullOrEmpty(status) && branch == 0 && emp != 0 && nature == 0)
            {
                model = model.Where(x => x.FileHandler.Id == emp).ToList();
            }
            //branch, source
            if (branch != 0 && source != 0 && emp == 0 && nature == 0 && string.IsNullOrEmpty(status))
            {
                model = model.Where(x => x.Branch.Id == branch && x.EnquirySource.Id == source).ToList();
            }
            //employee, source
            if (branch == 0 && source != 0 && emp != 0 && nature == 0 && string.IsNullOrEmpty(status))
            {
                model = model.Where(x => x.FileHandler.Id == emp && x.EnquirySource.Id == source).ToList();
            }
            //nature, source
            if (branch == 0 && source != 0 && emp == 0 && nature != 0 && string.IsNullOrEmpty(status))
            {
                model = model.Where(x => x.Nature.Id == nature && x.EnquirySource.Id == source).ToList();
            }
            // source, status
            if (!string.IsNullOrEmpty(status) && branch == 0 && emp == 0 && nature == 0 && source != 0)
            {
                string[] id = status.Split(',');
                model = model.Where(x => x.EnquirySource.Id == source && id.Contains(x.Status.Id.ToString())).ToList();
            }
            return model;
        }

        public IList<NBOModel> GetAllByFilter(List<NBOModel> model, string status, int branch, int emp, int nature)
        {
            //Branch
            if (string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature == 0)
            {
                model = model.Where(x => x.Branch.Id == branch).ToList();
            }
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature == 0)
            {
                string[] id = status.Split(',');
                model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch).ToList();
            }
            //status, Branch, Employee
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp != 0 && nature == 0)
            {
                string[] id = status.Split(',');
                model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.FileHandler.Id == emp).ToList();
            }
            //status, Branch, Nature, Employee
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp != 0 && nature != 0)
            {
                string[] id = status.Split(',');
                if (nature == 6)
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.FileHandler.Id == emp && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.FileHandler.Id == emp && x.Nature.Id == nature).ToList();
                }
            }
            //status, Branch, Nature
            if (!string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature != 0)
            {
                string[] id = status.Split(',');
                if (nature == 6)
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.Branch.Id == branch && x.Nature.Id == nature).ToList();
                }
            }
            //Branch, Nature
            if (string.IsNullOrEmpty(status) && branch != 0 && emp == 0 && nature != 0)
            {
                if (nature == 6)
                {
                    model = model.Where(x => x.Branch.Id == branch && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => x.Branch.Id == branch && x.Nature.Id == nature).ToList();
                }
            }
            // emp, nature
            if (string.IsNullOrEmpty(status) && branch == 0 && emp != 0 && nature != 0)
            {
                if (nature == 6)
                {
                    model = model.Where(x => x.FileHandler.Id == emp && (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => x.FileHandler.Id == emp && x.Nature.Id == nature).ToList();
                }
            }
            // Status, emp
            if (!string.IsNullOrEmpty(status) && branch == 0 && emp != 0 && nature == 0)
            {
                string[] id = status.Split(',');
                model = model.Where(x => id.Contains(x.Status.Id.ToString()) && x.FileHandler.Id == emp).ToList();
            }
            // Branch, emp
            if (string.IsNullOrEmpty(status) && branch != 0 && emp != 0 && nature == 0)
            {
                model = model.Where(x => x.Branch.Id == branch && x.FileHandler.Id == emp).ToList();
            }
            // nature, status
            if (!string.IsNullOrEmpty(status) && branch == 0 && emp == 0 && nature != 0)
            {
                string[] id = status.Split(',');
                if (nature == 6)
                {
                    model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2) && id.Contains(x.Status.Id.ToString())).ToList();
                }
                else
                {
                    model = model.Where(x => x.Nature.Id == nature && id.Contains(x.Status.Id.ToString())).ToList();
                }
            }
            // nature
            if (string.IsNullOrEmpty(status) && branch == 0 && emp == 0 && nature != 0)
            {
                if (nature == 6)
                {
                    model = model.Where(x => (x.Nature.Id == 1 || x.Nature.Id == 2)).ToList();
                }
                else
                {
                    model = model.Where(x => x.Nature.Id == nature).ToList();
                }
            }
            // employee
            if (string.IsNullOrEmpty(status) && branch == 0 && emp != 0 && nature == 0)
            {
                model = model.Where(x => x.FileHandler.Id == emp).ToList();
            }
            return model;
        }

        public override void Edit(NBOModel obj)
        {
            NBODAL dal = new NBODAL();
            INBO bl = dal.GetById(obj.Id);

            if (HttpContext.Current.User.IsInRole("Admin"))
            {
                bl.FileNumber = obj.FileNumber;

                IBusinessNature nat = new BusinessNature { Id = obj.NatureId };
                bl.Nature = nat;

                IBranch branch = new Branch { Id = obj.BranchId };
                bl.Branch = branch;

                IEmployee emp = new Employee { Id = obj.EmpId };
                bl.FileHandler = emp;

                IClient client = new Client { Id = obj.ClientId };
                bl.ClientName = client;

                ICountry cont = new Country { Id = obj.CountryId };
                bl.ClientCountry = cont;
            }

            bl.ContactName = obj.ContactName;
            bl.EmailId = obj.EmailId;
            bl.PhoneNo = obj.PhoneNo;
            bl.MobileNo = obj.MobileNo;
            bl.ContactId = obj.ContactId;
            bl.Fax = obj.Fax;
            bl.EventName = obj.EventName;
            bl.PaxNo = obj.PaxNo;
            bl.CheckinDate = obj.CheckinDate;
            bl.CheckOutDate = obj.CheckOutDate;
            bl.EventMonth = MonthYear(Convert.ToDateTime(obj.CheckinDate));
            bl.ContractCost = obj.ContractCost;
            bl.ContractValue = obj.ContractValue;
            bl.Remarks = obj.Remarks;
            IEnquirySource source = new EnquirySource { Id = obj.EnqSourceId };
            bl.EnquirySource = source;
            IEnquiryStatus status = new EnquiryStatus { Id = obj.StatusId };
            bl.Status = status;
            bl.StatusDate = obj.StatusDate;
            dal.InsertOrUpdate(bl);

            if (obj.StatusId == 4 || obj.StatusId == 5 || obj.StatusId == 6 || obj.StatusId == 7)
            {
                CopyNBO(obj, bl);
            }

        }

        public void CopyNBO(NBOModel obj, INBO bl1)
        {
            TempNBODAL dal = new TempNBODAL();
            var data = dal.GetAll().Where(x => x.NBOId == obj.Id && x.Status.Id == obj.StatusId).ToList();
            if (data.Count == 0)
            {
                ITempNBO bl = new TempNBO();
                bl.NBOId = obj.Id;
                bl.RequestMonth = bl1.RequestMonth;
                bl.RequestDate = bl1.RequestDate;
                bl.FileNumber = obj.FileNumber;
                bl.ContactName = obj.ContactName;
                bl.EmailId = obj.EmailId;
                bl.PhoneNo = obj.PhoneNo;
                bl.MobileNo = obj.MobileNo;
                bl.Fax = obj.Fax;
                bl.EventName = obj.EventName;
                bl.PaxNo = obj.PaxNo;
                bl.CheckinDate = obj.CheckinDate;
                bl.CheckOutDate = obj.CheckOutDate;
                bl.EventMonth = MonthYear(Convert.ToDateTime(obj.CheckinDate));
                bl.StatusDate = obj.StatusDate;
                bl.ContractValue = obj.ContractValue == null ? 0 : obj.ContractValue;
                bl.ContractCost = obj.ContractCost == null ? 0 : obj.ContractCost;
                bl.Remarks = obj.Remarks;

                IEmployee emp = new Employee { Id = obj.EmpId };
                bl.FileHandler = emp;
                IClient client = new Client { Id = obj.ClientId };
                bl.ClientName = client;
                ICountry cont = new Country { Id = obj.CountryId };
                bl.ClientCountry = cont;
                IBusinessNature nat = new BusinessNature { Id = obj.NatureId };
                bl.Nature = nat;
                IEnquirySource source = new EnquirySource { Id = obj.EnqSourceId };
                bl.EnquirySource = source;
                IEnquiryStatus status = new EnquiryStatus { Id = obj.StatusId };
                bl.Status = status;

                IBranch branch = new Branch { Id = obj.BranchId };
                bl.Branch = branch;

                dal.InsertOrUpdate(bl);
            }
        }

        public override void Insert(NBOModel obj)
        {
            NBODAL dal = new NBODAL();

            INBO bl = new NBO();
            bl.RequestMonth = MonthYear(Convert.ToDateTime(obj.RequestMonth));
            bl.RequestDate = obj.RequestDate;
            bl.FileNumber = obj.FileNumber;
            bl.ContactName = obj.ContactName;
            bl.EmailId = obj.EmailId;
            bl.PhoneNo = obj.PhoneNo;
            bl.MobileNo = obj.MobileNo;
            bl.Fax = obj.Fax;
            bl.ContactId = obj.ContactId;
            bl.ContactName = obj.ContactName;
            bl.EmailId = obj.EmailId;
            bl.PhoneNo = obj.PhoneNo;
            bl.MobileNo = obj.MobileNo;
            bl.Fax = obj.Fax;
            bl.EventName = obj.EventName;
            bl.PaxNo = obj.PaxNo;
            bl.CheckinDate = obj.CheckinDate;
            bl.CheckOutDate = obj.CheckOutDate;
            bl.EventMonth = MonthYear(Convert.ToDateTime(obj.CheckinDate));
            bl.StatusDate = obj.StatusDate;
            bl.ContractValue = obj.ContractValue == null ? 0 : obj.ContractValue;
            bl.ContractCost = obj.ContractCost == null ? 0 : obj.ContractCost;
            bl.Remarks = obj.Remarks;

            IEmployee emp = new Employee { Id = obj.EmpId };
            bl.FileHandler = emp;
            IClient client = new Client { Id = obj.ClientId };
            bl.ClientName = client;
            ICountry cont = new Country { Id = obj.CountryId };
            bl.ClientCountry = cont;
            IBusinessNature nat = new BusinessNature { Id = obj.NatureId };
            bl.Nature = nat;
            IEnquirySource source = new EnquirySource { Id = obj.EnqSourceId };
            bl.EnquirySource = source;
            IEnquiryStatus status = new EnquiryStatus { Id = obj.StatusId };
            bl.Status = status;

            IBranch branch = new Branch { Id = obj.BranchId };
            bl.Branch = branch;

            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            NBODAL dal = new NBODAL();
            INBO bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        private string MonthYear(DateTime dt)
        {
            string month = dt.Month >= 10 ? dt.Month.ToString() : "0" + dt.Month.ToString();
            string year = dt.Year.ToString();
            return month + year;
        }

        public IList<NBOModel> GetCashFlow()
        {
            List<NBOModel> model = GetAll().Where(x => x.Status.Id == 4 || x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7).ToList();
            return model;
        }

        public static bool ExistFile(string file)
        {
            NBODAL dal = new NBODAL();
            var record = dal.GetByFile(file.Trim()).ToList();
            if (record.Count == 0)
                return false;
            else
                return true;
        }

        #region -- NBO Comments ---

        //public void AddComments(NBOCommentsModel obj, int id)
        //{
        //    NBODAL dal = new NBODAL();
        //    dal.AddComments(obj, id);
        //}
        //
        //public IList<NBOCommentsModel> ListComments(int id)
        //{
        //    NBODAL dal = new NBODAL();
        //    AutoMapper.Mapper.CreateMap<NBOComments, NBOCommentsModel>();
        //    List<NBOCommentsModel> model = AutoMapper.Mapper.Map<List<NBOCommentsModel>>(dal.GetById(id).Comments);
        //    return model; ;
        //}
        #endregion

    }

}