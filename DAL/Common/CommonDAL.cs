using System;
using System.Collections.Generic;
using NHibernate;

namespace DAL.Common
{
    public abstract class CommonDAL<T>
    {
        public virtual T GetById(int id)
        {
            T obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(T))
            .Add(NHibernate.Criterion.Restrictions.Eq("Id", id))
            .SetFirstResult(0)
            .UniqueResult<T>();

            return obj;
        }

        public virtual IList<T> GetAll()
        {
            IList<T> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(T))
               .List<T>();
            return obj;
        }

        public void InsertOrUpdate(T obj)
        {
            ISession session = NHibernateHelper.OpenSession();
            using (ITransaction transcation = session.BeginTransaction())
            {
                try
                {
                    session.Flush();
                    session.SaveOrUpdate(obj);
                    transcation.Commit();
                }
                catch (Exception)
                {
                    transcation.Rollback();
                    throw;
                }
            }
        }

        public void InsertBulk(IList<T> obj)
        {
            ISession session = NHibernateHelper.OpenSession();
            using (ITransaction transcation = session.BeginTransaction())
            {
                try
                {
                    session.Flush();
                    foreach (var item in obj)
                    {
                        session.SaveOrUpdate(item);
                    }
                    transcation.Commit();

                }
                catch (Exception)
                {
                    transcation.Rollback();
                    throw;
                }
            }
        }

        public bool Delete(T obj)
        {
            bool flag;
            ISession session = NHibernateHelper.OpenSession();
            using (ITransaction transcation = session.BeginTransaction())
            {
                try
                {
                    session.Delete(obj);
                    transcation.Commit();
                    flag = true;
                }
                catch (Exception)
                {
                    transcation.Rollback();
                    flag = false;
                }

            }
            return flag;
        }
    }

    public static class MyExtension
    {
        public static string ToFinancialYear(this DateTime dateTime)
        {
            return (dateTime.Month >= 4 ? dateTime.ToString("yyyy") + "-" + dateTime.AddYears(1).ToString("yy") : dateTime.ToString("yyyy") + "-" + dateTime.AddYears(1).ToString("yy"));
        }

        public static string FyMonth(this DateTime dateTime)
        {
            string month = dateTime.Month < 10 ? '0' + dateTime.Month.ToString() : dateTime.Month.ToString();

            return month + "/" + dateTime.Year;
        }
    }

}
