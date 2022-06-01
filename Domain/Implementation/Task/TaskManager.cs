using System;
using System.Collections.Generic;
using Domain.Interface.Master;
using Domain.Interface.Task;

namespace Domain.Implementation.Task
{
    public class TaskManager : ITaskManager
    {
        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Type { get; set; }
        public virtual string Task { get; set; }
        public virtual IEmployee Assigneer { get; set; }
        public virtual string Notes { get; set; }
        public virtual int Status { get; set; }
        public virtual DateTime Start { get; set; }
        public virtual string StartTime { get; set; }
        public virtual DateTime? Due { get; set; }
        public virtual string DueTime { get; set; }
        public virtual DateTime? Compl { get; set; }
        public virtual string ComplTime { get; set; }
        public virtual int TaskRepeat { get; set; }
        public virtual IList<IEmployee> Contacts { get; set; }
        public virtual IList<IRepeatTask> RepeatTask { get; set; }

        public virtual string JobNumber { get; set; }
        public virtual string CompletePercentage { get; set; }

        public virtual string ClientName { get; set; }
        public virtual string EventName { get; set; }
        public virtual string TotalHours { get; set; }
        public virtual string ActualHours { get; set; }
        public virtual double? EmpCost { get; set; }
        public virtual double? TotalCost { get; set; }
        public virtual double? OtherCost { get; set; }
        public virtual double? GrandTotal { get; set; }
        public virtual ILocation Location { get; set; }
    }
}
