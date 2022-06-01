using System.Collections.Generic;
using DAL.Common;
using Domain.Interface.Task;
using NHibernate.Criterion;
using System;

namespace DAL.Task
{
    public class TaskManagerDAL : Common.CommonDAL<ITaskManager>
    {
        public IList<ITaskManager> GetByStatus(int StatusId)
        {
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .Add(NHibernate.Criterion.Restrictions.Eq("Status", StatusId))
               .List<ITaskManager>();
            return obj;
        }

        public IList<ITaskManager> GetByEmployee(int empid)
        {
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .Add(NHibernate.Criterion.Restrictions.Eq("Assigneer.Id", empid))
               .List<ITaskManager>();
            return obj;
        }

        public IList<ITaskManager> GetAllByEmp(int empid)
        {
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .CreateAlias("Contacts", "contacts")
                .Add(Restrictions.Eq("Assigneer.Id", empid) ||
                        Restrictions.Eq("contacts.Id", empid))
                   .SetResultTransformer(CriteriaSpecification.DistinctRootEntity)
                   .AddOrder(Order.Desc("Id"))
               .List<ITaskManager>();

            return obj;
        }

        public IList<ITaskManager> GetByEmployee(int empid, string date)
        {
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .Add(NHibernate.Criterion.Restrictions.Eq("Assigneer.Id", empid))
               .Add(NHibernate.Criterion.Restrictions.Eq("Start", System.Convert.ToDateTime(date)))
               .List<ITaskManager>();
            return obj;
        }

        public IList<ITaskManager> GetByEmployeePending(int empid)
        {
            int status = 3;
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .Add(NHibernate.Criterion.Restrictions.Eq("Assigneer.Id", empid))
               .Add(NHibernate.Criterion.Restrictions.Not(NHibernate.Criterion.Expression.Eq("Status", status)))
               .List<ITaskManager>();
            return obj;
        }

        public IList<ITaskManager> GetByAssigner(int empid)
        {
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .CreateAlias("Contacts", "emp")
               .Add(NHibernate.Criterion.Restrictions.Eq("emp.Id", empid))
               .List<ITaskManager>();
            return obj;
        }

        public IList<ITaskManager> GetByAssignerPending(int empid)
        {
            int status = 3;
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .CreateAlias("Contacts", "emp")
               .Add(NHibernate.Criterion.Restrictions.Eq("emp.Id", empid))
               .Add(NHibernate.Criterion.Restrictions.Not(NHibernate.Criterion.Expression.Eq("Status", status)))
               .List<ITaskManager>();
            return obj;
        }

        public IList<ITaskManager> GetByAssigner(int empid, string date)
        {
            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
               .CreateAlias("Contacts", "emp")
               .Add(NHibernate.Criterion.Restrictions.Eq("emp.Id", empid))
               .Add(NHibernate.Criterion.Restrictions.Eq("Start", System.Convert.ToDateTime(date)))
               .List<ITaskManager>();
            return obj;
        }

        public IList<ITaskManager> GetTodayTask(string date)
        {
            DateTime initDate = string.IsNullOrEmpty(date)?System.DateTime.Now.Date:Convert.ToDateTime(date);
            DateTime endDate = System.DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            IList<ITaskManager> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(ITaskManager))
                .Add(Restrictions.Eq("Start", initDate))
               .List<ITaskManager>();
            return obj;
        }
    }
}
