using System;
using Domain.Interface.Task;

namespace Domain.Implementation.Task
{
    public class RepeatTask : IRepeatTask
    {
        public virtual int Id { get; set; }
        public virtual DateTime TaskDate { get; set; }
        public virtual int Status { get; set; }
        public virtual string Notes { get; set; }
    }
}
