using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public sealed class INBOCommentsMap : ClassMap<NBOComments>
    {
        public INBOCommentsMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.UserName);
            Map(x => x.Date);
            Map(x => x.Comments);
            References<NBO>(x => x.NBO).Column("NboId").ForeignKey("fk_comments_nboid").LazyLoad();
        }
    }
}
