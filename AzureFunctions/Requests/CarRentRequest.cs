using System;

namespace AzureFunctions.Requests;

public class CarRentRequest
{
    public string Brand { get; set; }
    public string Model { get; set; }
}