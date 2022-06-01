using System;
using DAL.Common;
using Domain.Interface.CRM;
using NHibernate;

namespace DAL.CRM
{
    public class EmailTemplateDAL : Common.CommonDAL<IEmailTemplate>
    {
        public void AddAttachment(IEmailAttachment obj, int id)
        {
            IEmailTemplate bl = GetById(id);
            bl.Attachment.Add(obj);
            InsertOrUpdate(bl);
        }

        public bool DeleteEmailTemplate(IEmailTemplate obj)
        {
            bool flag;
            ISession session1 = NHibernateHelper.OpenSession();
            try
            {
                var q1 = "update MassEmailing set EmailTemplate_id = null  where EmailTemplate_id =" + obj.Id;
                var result1 = session1.CreateSQLQuery(q1).ExecuteUpdate();
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
