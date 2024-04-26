using System;
using System.Threading.Tasks;
using AzureFunctions.Requests;

namespace AzureFunctions.Services;

public interface IHotelBookingService : IRollbackBooking
{
    Task<Guid> BookHotel(UserInfoRequest userInfoRequest, HotelBookingRequest hotelBookingRequest);
}