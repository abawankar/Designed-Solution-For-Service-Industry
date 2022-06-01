
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation;
using Domain.Interface;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class AccountTypeModel : Domain.Implementation.AccountType
    {

    }

    public class AccountTypeRepository : Repository<AccountTypeModel>
    {

        public override AccountTypeModel GetById(int id)
        {
            AccountTypeDAL dal = new AccountTypeDAL();
            AutoMapper.Mapper.CreateMap<AccountType, AccountTypeModel>();
            AccountTypeModel model = AutoMapper.Mapper.Map<AccountTypeModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<AccountTypeModel> GetAll()
        {
            AccountTypeDAL dal = new AccountTypeDAL();
            AutoMapper.Mapper.CreateMap<AccountType, AccountTypeModel>();
            List<AccountTypeModel> model = AutoMapper.Mapper.Map<List<AccountTypeModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(AccountTypeModel obj)
        {
            AccountTypeDAL dal = new AccountTypeDAL();
            IAccountType bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(AccountTypeModel obj)
        {
            AccountTypeDAL dal = new AccountTypeDAL();
            IAccountType bl = new AccountType();
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            AccountTypeDAL dal = new AccountTypeDAL();
            IAccountType bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}