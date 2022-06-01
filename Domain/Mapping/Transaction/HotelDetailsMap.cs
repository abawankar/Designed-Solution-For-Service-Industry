using Domain.Implementation.Transaction;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Transaction
{
    public sealed class HotelDetailsMap : ClassMap<HotelDetails>
    {
        public HotelDetailsMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.HotelName);
            Map(x => x.NumberOfNight);
            Map(x => x.HotelBillingValue);
            References<NBO>(x => x.NBO).Column("NboId").ForeignKey("fk_hotel_nboid").LazyLoad();
        }
    }
}
