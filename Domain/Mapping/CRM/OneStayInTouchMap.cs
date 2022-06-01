using Domain.Implementation.CRM;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public class OneStayInTouchMap : ClassMap<OneStayInTouch>
    {
        public OneStayInTouchMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.CC);
            Map(x => x.BCC);
            Map(x => x.Subject);
            Map(x => x.Notes).CustomSqlType("text").Nullable();
            Map(x => x.Signature).CustomSqlType("text").Nullable();
            References<Contact>(x => x.Contact).LazyLoad();
            Map(x => x.Date);
        }
    }
}
