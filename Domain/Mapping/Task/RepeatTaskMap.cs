using Domain.Implementation.Task;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Task
{
    public class RepeatTaskMap : ClassMap<RepeatTask>
    {
        public RepeatTaskMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.TaskDate);
            Map(x => x.Status);
            Map(x => x.Notes);
        }
    }
}
