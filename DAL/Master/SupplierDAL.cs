using DAL.Common;
using Domain.Interface.Master;

namespace DAL.Master
{
    public class SupplierDAL : Common.CommonDAL<ISupplier>
    {
        public ISupplier GetByName(string name)
        {
            ISupplier obj = NHibernateHelper
            .OpenSession()
            .CreateCriteria(typeof(ISupplier))
            .Add(NHibernate.Criterion.Restrictions.Eq("Name", name))
            .SetFirstResult(0)
            .UniqueResult<ISupplier>();

            return obj;
        }
    }
}
