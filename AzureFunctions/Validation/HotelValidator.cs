using AzureFunctions.Requests;
using FluentValidation;

namespace AzureFunctions.Validation;

public class HotelValidator : AbstractValidator<HotelBookingRequest>
{
    public HotelValidator()
    {
        RuleFor(x => x.City).NotEqual("Zhytomyr");
    }
}