using System.Collections.Generic;
using DAL.Common;
using Domain.Interface.Transaction;
using NHibernate.Criterion;

namespace DAL.Transaction
{
    public class NBODAL : Common.CommonDAL<INBO>
    {
        public virtual IList<INBO> GetByFile(string file)
        {
            IList<INBO> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(INBO))
               .Add(NHibernate.Criterion.Restrictions.Eq("FileNumber", file))
               .List<INBO>();
            return obj;
        }

        public virtual IList<INBO> GetConfirmFile()
        {
            IList<INBO> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(INBO))
               .Add(Restrictions.Disjunction()
               .Add(Restrictions.Eq("Status.id", 4))
               .Add(NHibernate.Criterion.Restrictions.Eq("Status.Id", 5))
               ).List<INBO>();
            return obj;
        }

        public virtual IList<INBO> GetbyEmployee(int empid)
        {
            IList<INBO> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(INBO))
               .Add(NHibernate.Criterion.Restrictions.Eq("FileHandler.Id", empid))
               .List<INBO>();
            return obj;
        }

        public virtual IList<INBO> GetTop()
        {
            IList<INBO> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(INBO))
               .AddOrder(Order.Desc("Id"))
               .SetMaxResults(15)
               .List<INBO>();
            return obj;
        }
    }
}
