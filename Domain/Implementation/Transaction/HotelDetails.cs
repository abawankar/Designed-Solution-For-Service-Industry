using Domain.Interface.Transaction;

namespace Domain.Implementation.Transaction
{
    public class HotelDetails : IHotelDetails
    {
        public virtual int Id { get; set; }
        public virtual string HotelName { get; set; }
        public virtual string NumberOfNight { get; set; }
        public virtual string HotelBillingValue { get; set; }
        public virtual INBO NBO { get; set; }
    }
}
