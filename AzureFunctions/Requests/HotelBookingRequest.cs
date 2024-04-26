using System;
using FluentValidation;

namespace AzureFunctions.Requests;

public class HotelBookingRequest
{
    public string City { get; set; }
    public string Country { get; set; }
    public string HotelName { get; set; }
}