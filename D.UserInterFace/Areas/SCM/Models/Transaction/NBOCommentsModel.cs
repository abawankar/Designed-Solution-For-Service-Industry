using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Transaction;
using Domain.Implementation.Transaction;
using Domain.Interface.Transaction;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Transaction
{
    public class NBOCommentsModel : Domain.Implementation.Transaction.NBOComments
    {

    }

    public class NBOCommentsRepository : Repository<NBOCommentsModel>
    {

        public override NBOCommentsModel GetById(int id)
        {
            NBOCommentsDAL dal = new NBOCommentsDAL();
            AutoMapper.Mapper.CreateMap<NBOComments, NBOCommentsModel>();
            NBOCommentsModel model = AutoMapper.Mapper.Map<NBOCommentsModel>(dal.GetById(id));

            return model;
        }

        public override IList<NBOCommentsModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public override void Edit(NBOCommentsModel obj)
        {
            NBOCommentsDAL dal = new NBOCommentsDAL();
            INBOComments bl = dal.GetById(obj.Id);
            bl.Comments = obj.Comments;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(NBOCommentsModel obj)
        {
            throw new NotImplementedException();
        }

        public void AddComments(NBOCommentsModel obj, int nboid)
        {
            NBOCommentsDAL dal = new NBOCommentsDAL();
            INBOComments bl = new NBOComments();
            bl.Date = obj.Date;
            bl.Comments = obj.Comments;
            bl.UserName = obj.UserName;
            INBO nbo = new NBO { Id = nboid };
            bl.NBO = nbo;

            dal.InsertOrUpdate(bl);
        }

        public IList<NBOCommentsModel> GetNBOComments(int nboid)
        {
            NBOCommentsDAL dal = new NBOCommentsDAL();
            AutoMapper.Mapper.CreateMap<NBOComments, NBOCommentsModel>();
            List<NBOCommentsModel> model = AutoMapper.Mapper.Map<List<NBOCommentsModel>>(dal.GetAll().Where(x => x.NBO.Id == nboid));

            return model;
        }

        public override bool Delete(int id)
        {
            NBOCommentsDAL dal = new NBOCommentsDAL();
            INBOComments bl = dal.GetById(id);
            return dal.Delete(bl);
        }
    }
}