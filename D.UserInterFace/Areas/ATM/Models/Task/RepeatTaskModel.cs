
using D.UserInterFace.Models;
using DAL.Task;
using Domain.Interface.Task;

namespace D.UserInterFace.Areas.ATM.Models
{
    public class RepeatTaskModel : Domain.Implementation.Task.RepeatTask
    {
    }

    public class RepeatTaskRepository : Repository<RepeatTaskModel>
    {

        public override RepeatTaskModel GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public override System.Collections.Generic.IList<RepeatTaskModel> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public override void Edit(RepeatTaskModel obj)
        {
            RepeatTaskDAL dal = new RepeatTaskDAL();
            IRepeatTask bl = dal.GetById(obj.Id);
            bl.Status = obj.Status;
            bl.Notes = obj.Notes;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(RepeatTaskModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}