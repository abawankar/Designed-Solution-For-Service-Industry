
using Domain.Interface.Master;
namespace Domain.Implementation.Master
{
    public class ReportScheduler : IReportScheduler
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
