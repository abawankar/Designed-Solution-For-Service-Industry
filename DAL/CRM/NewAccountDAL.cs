using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DAL.Common;
using DAL.Master;
using Domain.Implementation.CRM;
using Domain.Interface;
using Domain.Interface.CRM;
using Domain.Interface.Transaction;
using NHibernate.Criterion;

namespace DAL.CRM
{
    public class NewAccountDAL : Common.CommonDAL<INewAccount>
    {
        public IList<INewAccount> GetByName(string name)
        {
            IList<INewAccount> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("AccountName", name))
                //.SetFirstResult(0)
                //.UniqueResult<INewAccount>();
            .List<INewAccount>();
            return obj;
        }

        public IList<INewAccount> GetIdList(int id)
        {
            IList<INewAccount> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("Id", id))
                //.SetFirstResult(0)
                //.UniqueResult<INewAccount>();
            .List<INewAccount>();
            return obj;
        }

        public IList<NewAccount> GetAccountName()
        {
            IList<NewAccount> list = NHibernateHelper
                .OpenSession()
                .CreateCriteria(typeof(INewAccount))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("AccountName"))
                .Add(Projections.Property("Id"))
                ).List<IList>()
                .Select(l => new NewAccount() { AccountName = (string)l[0], Id = (int)l[1] })
                .ToList();

            return list;
        }

        public IList<INewAccount> GetByCity(string city)
        {
            IList<INewAccount> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("City", city))
            .List<INewAccount>();
            return obj;
        }

        public IList<INewAccount> GetBySearch(string search)
        {
            IList<INewAccount> obj = NHibernateHelper
                            .OpenSession()
                            .CreateCriteria(typeof(INewAccount))
                            .CreateAlias("Industry", "ind")
                            .CreateAlias("Country", "cont")
                            .Add(NHibernate.Criterion.Restrictions.Like("AccountName", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Website", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Phone", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Fax", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Email", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Street", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("City", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("ind.Name", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("cont.Name", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("State", "%" + search + "%"))
                            .SetFirstResult(0)
                            .List<INewAccount>();
            return obj;
        }

        public IList<INewAccount> GetAccountByCountryid(int id)
        {
            IList<INewAccount> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("Country.Id", id))
            .List<INewAccount>();
            return obj;
        }

        public IList<INewAccount> GetByIndustryId(int id)
        {
            IList<INewAccount> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("Industry.Id", id))
            .List<INewAccount>();
            return obj;
        }

        public IList<INewAccount> GetAccountByType(string type)
        {
            IList<INewAccount> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("Type", type))
            .List<INewAccount>();
            return obj;
        }

        public IList<INewAccount> GetClientIdList(int id)
        {
            IList<INewAccount> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("AccountId", id))
                //.SetFirstResult(0)
                //.UniqueResult<INewAccount>();
            .List<INewAccount>();
            return obj;
        }

        public INewAccount GetByAcId(int actid)
        {
            INewAccount obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(INewAccount))
            .Add(NHibernate.Criterion.Restrictions.Eq("AccountId", actid))
            .SetFirstResult(0)
            .UniqueResult<INewAccount>();
            return obj;
        }

        public void AddContact(IContact obj, int id)
        {
            INewAccount bl = GetById(id);
            obj.AccountName = bl.AccountName;
            bl.Contact.Add(obj);
            InsertOrUpdate(bl);
        }

        public IList<INBO> GetAccountFiles(int id)
        {
            IList<INBO> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(INBO))
               .Add(Restrictions.Disjunction()
               .Add(Restrictions.Eq("ClientName.id", id))
               ).List<INBO>();
            return obj;
        }

        public void AddAccountType(int id, string typeList)
        {
            INewAccount bl = GetById(id);
            AccountTypeDAL dal = new AccountTypeDAL();
            string[] list = typeList.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int d = Convert.ToInt32(list[i]);
                IAccountType ac = dal.GetById(d);
                bl.ActType.Add(ac);
                InsertOrUpdate(bl);
            }
        }

        public virtual IList<INewAccount> GetByType(string type)
        {
            IList<INewAccount> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(INewAccount))
                .Add(NHibernate.Criterion.Restrictions.Eq("Type", type))
               .List<INewAccount>();
            return obj;
        }

        public virtual IList<INewAccount> GetTop()
        {
            IList<INewAccount> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(INewAccount))
               .AddOrder(Order.Desc("Id"))
               .SetMaxResults(10)
               .List<INewAccount>();
            return obj;
        }
    }
}
