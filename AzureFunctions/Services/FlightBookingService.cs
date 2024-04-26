using System;
using System.Threading.Tasks;
using AzureFunctions.Requests;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Services;

public class FlightBookingService : IFlightBookingService
{
    private readonly ILogger<HotelBookingService> _logger;
    private readonly IValidator<FlightBookingRequest> _validator;

    public FlightBookingService(ILogger<HotelBookingService> logger, IValidator<FlightBookingRequest> validator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    public async Task RollbackBooking(Guid reservationId)
    {
        _logger.LogInformation($"Rolling back booking of flight with id {reservationId}");
        await Task.Delay(5000);
    }

    public async Task<Guid> BookFlight(UserInfoRequest userInfoRequest, FlightBookingRequest flightBookingRequest)
    {
        _logger.LogInformation("Trying to book flight");
        await Task.Delay(5000);
        
        var id = Guid.NewGuid();
        var result = await _validator.ValidateAsync(flightBookingRequest);
        if (result.IsValid)
        {
            _logger.LogInformation($"Flight was booked successfully with id: {id}");
            return id;
        }
        _logger.LogWarning($"Validation was not passed when tried to book flight");
        return Guid.Empty;
    }
}