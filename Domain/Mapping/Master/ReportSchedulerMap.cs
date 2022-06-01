
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;
namespace Domain.Mapping.Master
{
    public class ReportSchedulerMap : ClassMap<ReportScheduler>
    {
        public ReportSchedulerMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
        }
    }
}
