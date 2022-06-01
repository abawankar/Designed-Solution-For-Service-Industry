using Domain.Implementation.Master;
using Domain.Implementation.Task;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Task
{
    public class TaskManagerMap : ClassMap<TaskManager>
    {
        public TaskManagerMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Date);
            Map(x => x.Type);
            Map(x => x.Task);
            Map(x => x.Notes);
            Map(x => x.Start);
            Map(x => x.StartTime);
            Map(x => x.Due);
            Map(x => x.DueTime);
            Map(x => x.Compl);
            Map(x => x.ComplTime);
            Map(x => x.Status);
            Map(x => x.TaskRepeat);
            Map(x => x.JobNumber);
            Map(x => x.CompletePercentage);
            Map(x => x.ClientName);
            Map(x => x.EventName);
            Map(x => x.TotalHours);
            Map(x => x.ActualHours);
            Map(x => x.EmpCost).CustomSqlType("numeric(12,2)").Not.Nullable();
            Map(x => x.TotalCost).CustomSqlType("numeric(12,2)").Not.Nullable();
            Map(x => x.OtherCost).CustomSqlType("numeric(12,2)").Not.Nullable();
            Map(x => x.GrandTotal).CustomSqlType("numeric(12,2)").Not.Nullable();
            References<Employee>(x => x.Assigneer).Column("EmpId").LazyLoad();
            References<Location>(x => x.Location).Column("LocationId").LazyLoad();
            HasManyToMany<Employee>(x => x.Contacts).ParentKeyColumn("TaskId").ChildKeyColumn("EmpId").LazyLoad();
            HasMany<RepeatTask>(x => x.RepeatTask).KeyColumn("TaskId").Cascade.All().LazyLoad();
        }
    }
}
