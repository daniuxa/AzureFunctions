using System;
using System.Threading.Tasks;
using AzureFunctions.Requests;

namespace AzureFunctions.Services;

public interface IFlightBookingService  : IRollbackBooking
{
    Task<Guid> BookFlight(UserInfoRequest userInfoRequest, FlightBookingRequest flightBookingRequest);
}