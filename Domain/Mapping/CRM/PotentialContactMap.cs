using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public sealed class PotentialContactMap : ClassMap<PotentialContact>
    {
        public PotentialContactMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Salutation);
            Map(x => x.FirstName);
            Map(x => x.MiddleName);
            Map(x => x.LastName);
            Map(x => x.Title);
            Map(x => x.Email);
            Map(x => x.Phone);
            Map(x => x.PhoneDirect);
            Map(x => x.Mobile);
            Map(x => x.Fax);
            Map(x => x.Messanger);
            Map(x => x.EmailOpt);
            Map(x => x.Department);
            Map(x => x.Street);
            Map(x => x.City);
            Map(x => x.StateProvince);
            Map(x => x.ZipPostalCode);
            References<Country>(x => x.Country).Column("CountryId").LazyLoad();
            Map(x => x.AccountName);
        }
    }
}
