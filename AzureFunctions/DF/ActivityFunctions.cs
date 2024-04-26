using System;
using System.Threading.Tasks;
using AzureFunctions.Requests;
using AzureFunctions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.DF;

public class ActivityFunctions
{
    private readonly IHotelBookingService _hotelBookingService;
    private readonly IFlightBookingService _flightBookingService;
    private readonly ICarRentService _carRentService;
    
    public ActivityFunctions(IHotelBookingService hotelBookingService, 
        IFlightBookingService flightBookingService, 
        ICarRentService carRentService)
    {
        _hotelBookingService = hotelBookingService;
        _flightBookingService = flightBookingService;
        _carRentService = carRentService;
    }
    
    [FunctionName("DFOrchestrationBooking_Flight")]
    public async Task<Guid> BookFlight([ActivityTrigger] (UserInfoRequest userInfoRequest, FlightBookingRequest flightBookingRequest) request,
        ILogger log)
    {
        var reservationId = await _flightBookingService.BookFlight(request.userInfoRequest, request.flightBookingRequest);
        
        return reservationId;
    }
    
    [FunctionName("DFOrchestrationBooking_Hotel")]
    public async Task<Guid> BookHotel([ActivityTrigger] (UserInfoRequest userInfoRequest, HotelBookingRequest hotelBookingRequest) request,
        ILogger log)
    {
        var reservationId = await _hotelBookingService.BookHotel(request.userInfoRequest, request.hotelBookingRequest);
        
        return reservationId;
    }
    
    [FunctionName("DFOrchestrationBooking_Car")]
    public async Task<Guid> RentCar([ActivityTrigger] (UserInfoRequest userInfoRequest, CarRentRequest carRentRequest) request,
        ILogger log)
    {
        var reservationId = await _carRentService.RentCarAsync(request.userInfoRequest, request.carRentRequest);
        
        return reservationId;
    }
    
    [FunctionName("DFOrchestrationRollingBack_Flight")]
    public async Task RollbackFlight([ActivityTrigger] Guid reservationId,
        ILogger log)
    {
        await _flightBookingService.RollbackBooking(reservationId);
    }
    
    [FunctionName("DFOrchestrationRollingBack_Hotel")]
    public async Task RollbackHotel([ActivityTrigger] Guid reservationId,
        ILogger log)
    {
        await _hotelBookingService.RollbackBooking(reservationId);
    }
}