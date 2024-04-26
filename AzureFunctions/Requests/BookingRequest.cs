namespace AzureFunctions.Requests;

public class BookingRequest
{
    public UserInfoRequest UserInfoRequest { get; set; }
    public FlightBookingRequest FlightBookingRequest { get; set; }
    public HotelBookingRequest HotelBookingRequest { get; set; }
    public CarRentRequest CarRentRequest { get; set; }
    public bool AllowCancelFlight { get; set; }
    public bool AllowCancelHotel { get; set; }
}