
using System.Collections.Generic;
using DAL.CRM;
using Domain.Implementation.CRM;
using Domain.Interface.CRM;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class AccountMailLogModel : Domain.Implementation.CRM.AccountMailLog
    {

    }

    public class AccountMailLogRepository : Repository<AccountMailLogModel>
    {

        public override AccountMailLogModel GetById(int id)
        {
            AccountMailLogDAL dal = new AccountMailLogDAL();
            AutoMapper.Mapper.CreateMap<AccountMailLog, AccountMailLogModel>();
            AccountMailLogModel model = AutoMapper.Mapper.Map<AccountMailLogModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<AccountMailLogModel> GetAll()
        {
            AccountMailLogDAL dal = new AccountMailLogDAL();
            AutoMapper.Mapper.CreateMap<AccountMailLog, AccountMailLogModel>();
            List<AccountMailLogModel> model = AutoMapper.Mapper.Map<List<AccountMailLogModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(AccountMailLogModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override void Insert(AccountMailLogModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override bool Delete(int id)
        {
            AccountMailLogDAL dal = new AccountMailLogDAL();
            IAccountMailLog bl = dal.GetById(id);
            bl.ContactList.Clear();
            dal.InsertOrUpdate(bl);
            return dal.Delete(bl);
        }

    }

}