using System;
using Domain.Interface.Master;

namespace DAL.Master
{
    public class BranchDAL : Common.CommonDAL<IBranch>
    {
        public void AddDepartment(IDepartment obj, int id)
        {
            IBranch bl = GetById(id);
            bl.Departments.Add(obj);
            InsertOrUpdate(bl);
        }

        public void AddNature(int branchId, string natureList)
        {
            IBranch bl = GetById(branchId);
            BusinessNatureDAL dal = new BusinessNatureDAL();
            string[] list = natureList.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int id = Convert.ToInt32(list[i]);
                IBusinessNature nature = dal.GetById(id);
                bl.Nature.Add(nature);
                InsertOrUpdate(bl);
            }
        }
    }
}
