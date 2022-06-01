using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DAL.Common;
using DAL.Master;
using Domain.Implementation.CRM;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using NHibernate;
using NHibernate.Criterion;

namespace DAL.CRM
{
    public class ContactDAL : Common.CommonDAL<IContact>
    {
        public void AddContactType(int id, string typeList)
        {
            IContact bl = GetById(id);
            ContactTypeDAL dal = new ContactTypeDAL();
            string[] list = typeList.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int d = Convert.ToInt32(list[i]);
                IContactType ac = dal.GetById(d);
                bl.ContType.Add(ac);
                InsertOrUpdate(bl);
            }
        }

        public IList<IContact> GetIdList(int id)
        {
            IList<IContact> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IContact))
            .Add(NHibernate.Criterion.Restrictions.Eq("Id", id))
            .List<IContact>();
            return obj;
        }

        public IList<IContact> GetByCity(string city)
        {
            IList<IContact> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IContact))
            .Add(NHibernate.Criterion.Restrictions.Eq("City", city))
            .List<IContact>();
            return obj;
        }

        public IList<IContact> GetByFirstName(string fName)
        {
            IList<IContact> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IContact))
            .Add(NHibernate.Criterion.Restrictions.Like("FirstName", "%" + fName + "%"))
            .List<IContact>();
            return obj;
        }

        public IList<IContact> GetByLastName(string lName)
        {
            IList<IContact> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IContact))
            .Add(NHibernate.Criterion.Restrictions.Like("LastName", "%" + lName + "%"))
            .List<IContact>();
            return obj;
        }

        public virtual IList<IContact> GetTop()
        {
            IList<IContact> obj = NHibernateHelper.OpenSession()
               .CreateCriteria(typeof(IContact))
               .AddOrder(Order.Desc("Id"))
               .SetMaxResults(10)
               .List<IContact>();
            return obj;
        }

        public IList<Contact> GetCity()
        {
            IList<Contact> list = NHibernateHelper
                .OpenSession()
                .CreateCriteria(typeof(INewAccount))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("City"))
                .Add(Projections.Property("Id"))
                ).List<IList>()
                .Select(l => new Contact() { City = (string)l[0], Id = (int)l[1] })
                .ToList();
            return list;
        }

        public IList<Contact> GetDuplicateMail()
        {
            IList<Contact> list = NHibernateHelper
                .OpenSession()
                .CreateCriteria(typeof(INewAccount))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("Email"))
                ).List<IList>()
                .Select(l => new Contact() { Email = (string)l[0]})
                .ToList();
            return list;
        }


        public IList<IContact> GetByCountryid(int id)
        {
            IList<IContact> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IContact))
            .Add(NHibernate.Criterion.Restrictions.Eq("Country.Id", id))
            .List<IContact>();
            return obj;
        }

        public IList<IContact> GetBySearch(string search)
        {
            IList<IContact> obj = NHibernateHelper
                            .OpenSession()
                            .CreateCriteria(typeof(IContact))
                            .CreateAlias("Country", "cont")
                            .Add(NHibernate.Criterion.Restrictions.Like("AccountName", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("FirstName", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("MiddleName", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("LastName", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Title", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Department", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Phone", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("PhoneDirect", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Mobile", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Messanger", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Email", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Messanger", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("Street", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("StateProvince", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("cont.Name", "%" + search + "%") ||
                            NHibernate.Criterion.Restrictions.Like("City", "%" + search + "%"))
                            .SetFirstResult(0)
                            .List<IContact>();
            return obj;
        }

        public IList<IContact> GetByContactType(int id)
        {
            IList<IContact> obj = NHibernateHelper
                            .OpenSession()
                            .CreateCriteria(typeof(IContact))
                            .CreateAlias("ContType", "cont")
                            .Add(NHibernate.Criterion.Restrictions.Eq("cont.Id", id))
                            .SetFirstResult(0)
                            .List<IContact>();
            return obj;
        }

        public bool DeleteContact(IContact obj)
        {
            bool flag;
            ISession session1 = NHibernateHelper.OpenSession();
            try
            {
                var q1 = "delete OneStayInTouch where Contact_id =" + obj.Id;
                var q2 = "delete ContactMassEmail where Contact_id = " + obj.Id;
                var q3 = "delete GroupContactList where Contact_id =" + obj.Id;
                var q4 = "delete TypeOfContact where Contact_id=" + obj.Id;
                var q5 = "delete AccountMailContact where Contact_id=" + obj.Id;
                var result1 = session1.CreateSQLQuery(q1).ExecuteUpdate();
                var result2 = session1.CreateSQLQuery(q2).ExecuteUpdate();
                var result3 = session1.CreateSQLQuery(q3).ExecuteUpdate();
                var result4 = session1.CreateSQLQuery(q4).ExecuteUpdate();
                var result5 = session1.CreateSQLQuery(q5).ExecuteUpdate();
            }
            catch (Exception)
            {
                flag = false;
            }

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
}
