using Domain.Implementation.Master;
using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public sealed class NBOMap : ClassMap<NBO>
    {
        public NBOMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
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
            Map(x => x.ContactId);
            References<Employee>(x => x.FileHandler).Column("EmpId").ForeignKey("fk_nbo_EmpId").LazyLoad();
            References<Client>(x => x.ClientName).Column("ClientId").ForeignKey("fk_nbo_CliendId").LazyLoad();
            References<Country>(x => x.ClientCountry).Column("ClientCountId").ForeignKey("fk_nbo_ClientCountId").LazyLoad();
            References<BusinessNature>(x => x.Nature).Column("NatureId").ForeignKey("fk_nbo_natureid").LazyLoad();
            References<EnquirySource>(x => x.EnquirySource).Column("EnqSourceId").ForeignKey("fk_nbo_EnqSourceId").LazyLoad();
            References<EnquiryStatus>(x => x.Status).Column("StatusId").ForeignKey("fk_nbo_StatusId").LazyLoad();
            Map(x => x.StatusDate);
            Map(x => x.ContractValue).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.ContractCost).CustomSqlType("numeric(12, 2)").Not.Nullable();
            Map(x => x.Remarks).CustomSqlType("text").Nullable();
            References<Branch>(x => x.Branch).Column("BranchId").ForeignKey("fk_nbo_branchid").LazyLoad();

        }
    }
}
