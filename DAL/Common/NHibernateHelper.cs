using System;
using System.Web;
using Domain.Implementation.Master;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Context;

namespace DAL.Common
{
    public sealed class NHibernateHelper
    {
        public static string SqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private static NHibernate.ISessionFactory _ISessionFactory;

        private static NHibernate.ISessionFactory SessionFactory
        {
            get
            {
                try
                {
                    if (_ISessionFactory == null)
                    {
                        _ISessionFactory = Fluently.Configure()
                            .Database(MsSqlConfiguration.MsSql2012.ConnectionString(SqlConnection))

                            .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(Company).Assembly))
                            .ExposeConfiguration(c =>
                            {
                                c.Properties.Add("current_session_context_class", "managed_web");
                            })
                            .BuildSessionFactory();
                    }

                    return _ISessionFactory;
                }
                catch (Exception ex) { throw ex; }
            }
        }

        public static NHibernate.ISession OpenSession()
        {
            if (System.Web.HttpContext.Current != null)
            {
                if (!ManagedWebSessionContext.HasBind(System.Web.HttpContext.Current, _ISessionFactory))
                {
                    ManagedWebSessionContext.Bind(HttpContext.Current, SessionFactory.OpenSession());
                }
            }
            else
            {
                return SessionFactory.OpenSession();
            }

            return SessionFactory.GetCurrentSession();
        }
    }
}
