using Domain.Implementation.Master;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Mapping.Master
{
    public class EmpRightsMap : ClassMap<EmpRights>
    {
        public EmpRightsMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Code);
            Map(x => x.Area);
            Map(x => x.MnuName);
            Map(x => x.Operation);
            Map(x => x.Description);
        }
    }
}
