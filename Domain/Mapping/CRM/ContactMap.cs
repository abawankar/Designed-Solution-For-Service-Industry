using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.CRM
{
    public sealed class ContactMap : ClassMap<Contact>
    {
        public ContactMap()
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
            References<Country>(x => x.Country).Column("CountryId").ForeignKey("fk_contact_contid").LazyLoad();
            Map(x => x.AccountName);
            HasManyToMany<ContactType>(x => x.ContType).Table("TypeOfContact").Cascade.All().LazyLoad();

            Map(x => x.CreationDate);
            Map(x => x.ModifyDate);
            References<Employee>(x => x.Creater).Column("Creator").LazyLoad();
            References<Employee>(x => x.Modifyby).Column("Modifier").LazyLoad();
        }
    }
}
