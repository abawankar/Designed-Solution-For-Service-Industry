using Domain.Interface.Master;
using FluentNHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace DAL.Common
{
    public static class CreateDB
    {
        public static void CreateDatabase(string connectionString)
        {
            NHibernate.Cfg.Configuration config = Fluently.Configure().Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(c => c.Is(connectionString)))
                 .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(ICompany).Assembly)).CurrentSessionContext<NHibernate.Context.ThreadStaticSessionContext>().BuildConfiguration();

            var schemaExport = new SchemaExport(config);

            schemaExport.Create(false, true);
        }
    }
}
