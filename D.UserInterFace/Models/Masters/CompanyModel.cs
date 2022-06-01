
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;

namespace D.UserInterFace.Models.Masters
{
    public class CompanyModel : Domain.Implementation.Master.Company
    {
    }

    public class CompanyRepository : Repository<CompanyModel>
    {

        //Implmentaion of company model abstract class
        #region ----- Company Model ----
        public override CompanyModel GetById(int id)
        {
            CompanyDAL dal = new CompanyDAL();
            AutoMapper.Mapper.CreateMap<Company, CompanyModel>();
            CompanyModel model = AutoMapper.Mapper.Map<CompanyModel>(dal.GetById(id));

            return model;
        }

        public override IList<CompanyModel> GetAll()
        {
            CompanyDAL dal = new CompanyDAL();
            AutoMapper.Mapper.CreateMap<Company, CompanyModel>();
            List<CompanyModel> model = AutoMapper.Mapper.Map<List<CompanyModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(CompanyModel obj)
        {
            CompanyDAL dal = new CompanyDAL();
            ICompany bl = dal.GetById(obj.Id);
            bl.Code = obj.Code;
            bl.Name = obj.Name;
            bl.Address = obj.Address;
            bl.PhoneNo = obj.PhoneNo;
            bl.FaxNo = obj.FaxNo;
            dal.InsertOrUpdate(bl);

        }

        public override void Insert(CompanyModel obj)
        {
            CompanyDAL dal = new CompanyDAL();
            ICompany bl = new Company();
            bl.Code = obj.Code;
            bl.Name = obj.Name;
            bl.Address = obj.Address;
            bl.PhoneNo = obj.PhoneNo;
            bl.FaxNo = obj.FaxNo;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        //Add Branches to existing company
        #region ---- Company Branches ----

        public void AddBranches(BranchModel obj, int compId)
        {
            CompanyDAL dal = new CompanyDAL();
            IBranch bl = new Branch();
            bl.Code = obj.Code;
            bl.Name = obj.Name;
            bl.Address = obj.Address;
            bl.PhoneNo = obj.PhoneNo;
            bl.FaxNo = obj.FaxNo;
            dal.AddBranch(bl, compId);
        }

        public void AddNature(int branchId, string naturelist)
        {
            BranchDAL dal = new BranchDAL();
            dal.AddNature(branchId, naturelist);
        }

        public void EditBranches(BranchModel obj)
        {
            BranchDAL dal = new BranchDAL();
            IBranch bl = dal.GetById(obj.Id);
            bl.Code = obj.Code;
            bl.Name = obj.Name;
            bl.Address = obj.Address;
            bl.PhoneNo = obj.PhoneNo;
            bl.FaxNo = obj.FaxNo;
            dal.InsertOrUpdate(bl);
        }

        public IList<BranchModel> BranchList(int compId)
        {
            CompanyDAL dal = new CompanyDAL();
            AutoMapper.Mapper.CreateMap<Branch, BranchModel>();
            List<BranchModel> model = AutoMapper.Mapper.Map<List<BranchModel>>(dal.GetById(compId).Branches);

            return model;
        }

        public IList<BranchModel> SelectBranch(int compId)
        {
            CompanyDAL dal = new CompanyDAL();
            AutoMapper.Mapper.CreateMap<Branch, BranchModel>();
            List<BranchModel> model = AutoMapper.Mapper.Map<List<BranchModel>>(dal.GetById(compId).Branches);
            model.Add(new BranchModel { Id = 0, Name = "* Select *" });
            return model;
        }

        //Get all branch list
        public IList<BranchModel> BranchList()
        {
            BranchDAL dal = new BranchDAL();
            AutoMapper.Mapper.CreateMap<Branch, BranchModel>();
            List<BranchModel> model = AutoMapper.Mapper.Map<List<BranchModel>>(dal.GetAll());

            return model;
        }

        public BranchModel BranchById(int id)
        {
            BranchDAL dal = new BranchDAL();
            AutoMapper.Mapper.CreateMap<Branch, BranchModel>();
            BranchModel model = AutoMapper.Mapper.Map<BranchModel>(dal.GetById(id));

            return model;
        }

        #endregion

        //Add Department to existing Branches
        #region ---- Branches Department ----

        public void AddDepartment(DepartmentModel obj, int branId)
        {
            BranchDAL dal = new BranchDAL();
            IDepartment bl = new Department();
            bl.Code = obj.Code;
            bl.Name = obj.Name;
            dal.AddDepartment(bl, branId);
        }

        public void EditDepartment(DepartmentModel obj)
        {
            DepartmentDAL dal = new DepartmentDAL();
            IDepartment bl = dal.GetById(obj.Id);
            bl.Code = obj.Code;
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public IList<DepartmentModel> DepartmentList(int branId)
        {
            BranchDAL dal = new BranchDAL();
            AutoMapper.Mapper.CreateMap<Department, DepartmentModel>();
            List<DepartmentModel> model = AutoMapper.Mapper.Map<List<DepartmentModel>>(dal.GetById(branId).Departments);

            return model;
        }

        public IList<DepartmentModel> DepartmentList()
        {
            DepartmentDAL dal = new DepartmentDAL();
            AutoMapper.Mapper.CreateMap<Department, DepartmentModel>();
            List<DepartmentModel> model = AutoMapper.Mapper.Map<List<DepartmentModel>>(dal.GetAll());

            return model;
        }

        #endregion

        public static string BranchName(int id)
        {
            if (id == 0)
            {
                return "All";
            }
            else
            {
                CompanyRepository c = new CompanyRepository();
                return c.BranchById(id).Name;
            }
        }
    }

}