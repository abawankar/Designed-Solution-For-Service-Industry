
namespace Domain.Interface.Transaction
{
    public interface IHotelDetails
    {
        int Id { get; set; }
        string HotelName { get; set; }
        string NumberOfNight { get; set; }
        string HotelBillingValue { get; set; }
        INBO NBO { get; set; }
    }
}
