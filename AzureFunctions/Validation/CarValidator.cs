using AzureFunctions.Requests;
using FluentValidation;

namespace AzureFunctions.Validation;

public class CarValidator : AbstractValidator<CarRentRequest>
{
    public CarValidator()
    {
        RuleFor(x => x.Brand).NotEqual("BMW");
    }
}