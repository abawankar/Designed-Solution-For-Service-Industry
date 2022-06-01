using System.Collections.Generic;
using DAL.Common;
using Domain.Interface.Master;

namespace DAL.Master
{
    public class ClientDAL : Common.CommonDAL<IClient>
    {
        public IClient GetByName(string name)
        {
            IClient obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IClient))
            .Add(NHibernate.Criterion.Restrictions.Eq("Name", name))
            .SetFirstResult(0)
            .UniqueResult<IClient>();

            return obj;
        }

        public IList<IClient> GetByNameList(string name)
        {
            IList<IClient> obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(IClient))
            .Add(NHibernate.Criterion.Restrictions.Eq("Name", name))
            .List<IClient>();

            return obj;
        }
    }
}
