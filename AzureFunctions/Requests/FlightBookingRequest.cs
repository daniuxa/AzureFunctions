using System;
using FluentValidation;

namespace AzureFunctions.Requests;

public class FlightBookingRequest
{
    public string AirportFrom { get; set; }
    public string AirportTo { get; set; }
}