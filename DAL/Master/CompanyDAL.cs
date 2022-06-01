using Domain.Interface.Master;

namespace DAL.Master
{
    public class CompanyDAL : Common.CommonDAL<ICompany>
    {
        public void AddBranch(IBranch obj, int id)
        {
            ICompany bl = GetById(id);
            bl.Branches.Add(obj);
            InsertOrUpdate(bl);
        }
    }
}
