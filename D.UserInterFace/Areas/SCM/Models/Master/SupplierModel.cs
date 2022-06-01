
using System.Collections.Generic;
using System.Linq;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Master
{
    public class SupplierModel : Domain.Implementation.Master.Supplier
    {
        public int CatId { get; set; }
    }


    public class SupplierRepository : Repository<SupplierModel>
    {

        public override SupplierModel GetById(int id)
        {
            SupplierDAL dal = new SupplierDAL();
            AutoMapper.Mapper.CreateMap<Supplier, SupplierModel>();
            AutoMapper.Mapper.CreateMap<Supplier, SupplierModel>()
            .ForMember(dest => dest.CatId, opt => opt.MapFrom(scr => scr.Category.Id));
            SupplierModel model = AutoMapper.Mapper.Map<SupplierModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<SupplierModel> GetAll()
        {
            SupplierDAL dal = new SupplierDAL();
            AutoMapper.Mapper.CreateMap<Supplier, SupplierModel>();
            AutoMapper.Mapper.CreateMap<Supplier, SupplierModel>()
            .ForMember(dest => dest.CatId, opt => opt.MapFrom(scr => scr.Category.Id));
            List<SupplierModel> model = AutoMapper.Mapper.Map<List<SupplierModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(SupplierModel obj)
        {
            SupplierDAL dal = new SupplierDAL();
            ISupplier bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            bl.AVLCode = obj.AVLCode;

            ISupplierCategory cat = new SupplierCategory { Id = obj.CatId };
            bl.Category = cat;

            dal.InsertOrUpdate(bl);
        }

        public override void Insert(SupplierModel obj)
        {
            SupplierDAL dal = new SupplierDAL();
            ISupplier bl = new Supplier();
            bl.Name = obj.Name;

            bl.AVLCode = obj.AVLCode;

            ISupplierCategory cat = new SupplierCategory { Id = obj.CatId };
            bl.Category = cat;

            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public static string SupplierName(string name)
        {
            SupplierRepository c = new SupplierRepository();
            string supplier = "";
            if (string.IsNullOrEmpty(name))
            {
                return supplier = "All";
            }
            else
            {
                string[] id = name.Split(',');
                foreach (var item in c.GetAll().Where(x => id.Contains(x.Id.ToString())))
                {
                    if (supplier == "")
                    {
                        supplier = item.Name;
                    }
                    else
                    {
                        supplier = supplier + "/" + item.Name;
                    }
                }
                return supplier;
            }
        }

        public static int GetByName(string name)
        {
            SupplierDAL dal = new SupplierDAL();
            return dal.GetByName(name).Id;
        }
    }
}