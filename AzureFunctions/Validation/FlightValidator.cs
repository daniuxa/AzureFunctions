using AzureFunctions.Requests;
using FluentValidation;

namespace AzureFunctions.Validation;

public class FlightValidator : AbstractValidator<FlightBookingRequest>
{
    public FlightValidator()
    {
        RuleFor(x => x.AirportFrom).NotEqual(y => y.AirportTo);
    }
}