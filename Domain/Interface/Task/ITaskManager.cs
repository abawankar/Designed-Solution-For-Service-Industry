using System;
using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Interface.Task
{
    public interface ITaskManager
    {
        int Id { get; set; }
        DateTime Date { get; set; }
        string Type { get; set; }
        string Task { get; set; }
        IEmployee Assigneer { get; set; }
        string Notes { get; set; }
        int Status { get; set; }
        DateTime Start { get; set; }
        string StartTime { get; set; }
        DateTime? Due { get; set; }
        string DueTime { get; set; }
        DateTime? Compl { get; set; }
        string ComplTime { get; set; }
        int TaskRepeat { get; set; }
        IList<IEmployee> Contacts { get; set; }
        IList<IRepeatTask> RepeatTask { get; set; }
        string JobNumber { get; set; }
        string CompletePercentage { get; set; }
        string ClientName { get; set; }
        string EventName { get; set; }
        string TotalHours { get; set; }
        string ActualHours { get; set; }
        double? EmpCost { get; set; }
        double? TotalCost { get; set; }
        double? OtherCost { get; set; }
        double? GrandTotal { get; set; }
        ILocation Location { get; set; }
    }
}
