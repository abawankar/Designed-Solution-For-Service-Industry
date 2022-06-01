
using DAL.Master;
using Domain.Interface.Master;
namespace D.UserInterFace.Models.Masters
{
    public class ReportSchedulerModel : Domain.Implementation.Master.ReportScheduler
    {
    }

    public class ReportSchedulerRepository : Repository<ReportSchedulerModel>
    {

        public override ReportSchedulerModel GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public override System.Collections.Generic.IList<ReportSchedulerModel> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public override void Edit(ReportSchedulerModel obj)
        {
            ReportSchedulerDAL dal = new ReportSchedulerDAL();
            IReportScheduler bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(ReportSchedulerModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override bool Delete(int id)
        {
            ReportSchedulerDAL dal = new ReportSchedulerDAL();
            IReportScheduler bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}