using System;
using System.Threading.Tasks;
using AzureFunctions.Requests;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Services;

public class HotelBookingService : IHotelBookingService
{
    private readonly ILogger<HotelBookingService> _logger;
    private readonly IValidator<HotelBookingRequest> _validator;

    public HotelBookingService(ILogger<HotelBookingService> logger, IValidator<HotelBookingRequest> validator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    public async Task RollbackBooking(Guid reservationId)
    {
        _logger.LogInformation($"Rolling back booking of hotel with id {reservationId}");
        await Task.Delay(5000);
    }

    public async Task<Guid> BookHotel(UserInfoRequest userInfoRequest, HotelBookingRequest hotelBookingRequest)
    {
        await Task.Delay(5000);
        var id = Guid.NewGuid();
        var result = await _validator.ValidateAsync(hotelBookingRequest);
        if (result.IsValid)
        {
            _logger.LogInformation($"Hotel was booked successfully with id: {id}");
            return id;
        }
        _logger.LogWarning($"Validation was not passed when tried to book hotel");
        return Guid.Empty;
    }
}