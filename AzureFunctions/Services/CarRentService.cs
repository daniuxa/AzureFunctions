using System;
using System.Threading.Tasks;
using AzureFunctions.Requests;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Services;

public class CarRentService : ICarRentService
{
    private readonly ILogger<CarRentService> _logger;
    private readonly IValidator<CarRentRequest> _validator;

    public CarRentService(ILogger<CarRentService> logger, IValidator<CarRentRequest> validator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task RollbackBooking(Guid reservationId)
    {
        _logger.LogInformation($"Rolling back rent of car with id {reservationId}");
        await Task.Delay(5000);
    }

    public async Task<Guid> RentCarAsync(UserInfoRequest userInfoRequest, CarRentRequest carRentRequest)
    {
        await Task.Delay(5000);
        var id = Guid.NewGuid();
        var result = await _validator.ValidateAsync(carRentRequest);
        if (result.IsValid)
        {
            _logger.LogInformation($"Car was rent successfully with id: {id}");
            return id;
        }
        _logger.LogWarning("Validation was not passed when tried to rent car");
        return Guid.Empty;
    }
}