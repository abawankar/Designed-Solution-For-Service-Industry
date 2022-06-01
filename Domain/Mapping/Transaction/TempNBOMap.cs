using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public sealed class TempNBOMap : ClassMap<TempNBO>
    {
        public TempNBOMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.NBOId);
            Map(x => x.RequestMonth);
            Map(x => x.RequestDate);
            Map(x => x.FileNumber);
            Map(x => x.ContactName);
            Map(x => x.EmailId);
            Map(x => x.PhoneNo);
            Map(x => x.MobileNo);
            Map(x => x.Fax);
            Map(x => x.EventName);
            Map(x => x.PaxNo);
            Map(x => x.CheckinDate);
            Map(x => x.CheckOutDate);
            Map(x => x.EventMonth);
            References<Employee>(x => x.FileHandler).Column("EmpId").LazyLoad();
            References<Client>(x => x.ClientName).Column("ClientId").LazyLoad();
            References<Country>(x => x.ClientCountry).Column("ClientCountId").LazyLoad();
            References<BusinessNature>(x => x.Nature).Column("NatureId").LazyLoad();
            References<EnquirySource>(x => x.EnquirySource).Column("EnqSourceId").LazyLoad();
            References<EnquiryStatus>(x => x.Status).Column("StatusId").LazyLoad();
            Map(x => x.StatusDate);
            Map(x => x.ContractValue).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.ContractCost).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.Remarks).CustomSqlType("text").Nullable();
            References<Branch>(x => x.Branch).Column("BranchId").LazyLoad();
        }
    }
}
