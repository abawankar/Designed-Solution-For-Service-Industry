
using System.Collections.Generic;
using System.Linq;
using DAL.Transaction;
using Domain.Implementation.Transaction;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Transaction
{
    public class TempNBOModel : Domain.Implementation.Transaction.TempNBO
    {
        public int NatureId { get; set; }
        public int EmpId { get; set; }
        public int ClientId { get; set; }
        public int CountryId { get; set; }
        public int EnqSourceId { get; set; }
        public int StatusId { get; set; }
        public int BranchId { get; set; }
    }

    public class TempNBORepository : Repository<TempNBOModel>
    {

        public override TempNBOModel GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public override System.Collections.Generic.IList<TempNBOModel> GetAll()
        {
            TempNBODAL dal = new TempNBODAL();
            AutoMapper.Mapper.CreateMap<TempNBO, TempNBOModel>();
            AutoMapper.Mapper.CreateMap<TempNBO, TempNBOModel>()
                .ForMember(dest => dest.NatureId, opt => opt.MapFrom(scr => scr.Nature.Id))
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.FileHandler.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(scr => scr.ClientName.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.ClientCountry.Id))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(scr => scr.Branch.Id))
                .ForMember(dest => dest.EnqSourceId, opt => opt.MapFrom(scr => scr.EnquirySource.Id))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(scr => scr.Status.Id));
            List<TempNBOModel> model = AutoMapper.Mapper.Map<List<TempNBOModel>>(dal.GetAll().OrderByDescending(x => x.Id));

            return model;
        }

        public override void Edit(TempNBOModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override void Insert(TempNBOModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }

}