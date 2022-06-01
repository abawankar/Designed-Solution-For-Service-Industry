using System;
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Master
{
    public class SupplierCategoryModel : Domain.Implementation.Master.SupplierCategory
    {
    }

    public class SupplierCategoryRepository : Repository<SupplierCategoryModel>
    {
        public override SupplierCategoryModel GetById(int id)
        {
            SupplierCategoryDAL dal = new SupplierCategoryDAL();
            AutoMapper.Mapper.CreateMap<SupplierCategory, SupplierCategoryModel>();
            SupplierCategoryModel model = AutoMapper.Mapper.Map<SupplierCategoryModel>(dal.GetById(id));

            return model;
        }

        public override IList<SupplierCategoryModel> GetAll()
        {
            SupplierCategoryDAL dal = new SupplierCategoryDAL();
            AutoMapper.Mapper.CreateMap<SupplierCategory, SupplierCategoryModel>();
            List<SupplierCategoryModel> model = AutoMapper.Mapper.Map<List<SupplierCategoryModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(SupplierCategoryModel obj)
        {
            SupplierCategoryDAL dal = new SupplierCategoryDAL();
            ISupplierCategory bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(SupplierCategoryModel obj)
        {
            SupplierCategoryDAL dal = new SupplierCategoryDAL();
            ISupplierCategory bl = new SupplierCategory();
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}