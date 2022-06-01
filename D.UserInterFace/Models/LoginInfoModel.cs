using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace D.UserInterFace.Models
{
    public class LoginInfoModel : LoginInfo
    {
        public string Time { get; set; }
        public string Name { get; set; }
    }

    public class LoginInfoRepository : Repository<LoginInfoModel>
    {
        public override LoginInfoModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override IList<LoginInfoModel> GetAll()
        {
            LoginInfoDAL dal = new LoginInfoDAL();
            AutoMapper.Mapper.CreateMap<LoginInfo, LoginInfoModel>();
            AutoMapper.Mapper.CreateMap<LoginInfo, LoginInfoModel>()
               .ForMember(dest => dest.Time, opt => opt.MapFrom(scr => scr.Date.ToLongTimeString()));
            List<LoginInfoModel> model = AutoMapper.Mapper.Map<List<LoginInfoModel>>(dal.GetAll());
            return model;
        }

        public override void Edit(LoginInfoModel obj)
        {
            throw new NotImplementedException();
        }

        public override void Insert(LoginInfoModel obj)
        {
            throw new NotImplementedException();
        }

        public static void Insert(string username)
        {
            LoginInfoDAL dal = new LoginInfoDAL();
            ILoginInfo bl = new LoginInfo();
            bl.UserName = username;
            bl.Date = System.DateTime.Today + DateTime.Now.TimeOfDay;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            LoginInfoDAL dal = new LoginInfoDAL();
            ILoginInfo bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}