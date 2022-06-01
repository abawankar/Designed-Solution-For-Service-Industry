using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Interface.Task
{
    public interface IRepeatTask
    {
        int Id { get; set; }
        DateTime TaskDate { get; set; }
        int Status { get; set; }
        string Notes { get; set; }
    }
}
