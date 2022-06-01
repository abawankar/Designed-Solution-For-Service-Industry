using DAL.Common;
using Domain.Interface.Master;
using System;
using System.Collections.Generic;

namespace DAL.Master
{
    public class EmployeeDAL : Common.CommonDAL<IEmployee>
    {
        public IEmployee GetByAppLogin(string name)
        {
            IEmployee obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IEmployee))
            .Add(NHibernate.Criterion.Restrictions.Eq("AppLogin", name))
            .SetFirstResult(0)
            .UniqueResult<IEmployee>();

            return obj;
        }

        public IEmployee GetByName(string name)
        {
            IEmployee obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IEmployee))
            .Add(NHibernate.Criterion.Restrictions.Eq("EmpName", name))
            .SetFirstResult(0)
            .UniqueResult<IEmployee>();

            return obj;
        }

        public  IList<IEmployee> GetEmpById(int id)
        {
            IList<IEmployee> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(IEmployee))
                .Add(NHibernate.Criterion.Restrictions.Eq("Id", id))
               .List<IEmployee>();
            return obj;
        }

        public void AddRights(int empId, string rightsList)
        {
            IEmployee bl = GetById(empId);
            EmpRightsDAL dal = new EmpRightsDAL();
            string[] rightsid = rightsList.Split(',');
            List<IEmpRights> list = new List<IEmpRights>();
            for (int i = 1; i < rightsid.Length; i++)
            {
                int id = Convert.ToInt32(rightsid[i]);
                IEmpRights empRights = dal.GetById(id);
                bl.EmpRights.Add(empRights);
            }
            InsertOrUpdate(bl);
        }
    }
}
