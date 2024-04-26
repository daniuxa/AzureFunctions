using System;
using System.Threading.Tasks;
using AzureFunctions.Requests;

namespace AzureFunctions.Services;

public interface ICarRentService : IRollbackBooking
{
    Task<Guid> RentCarAsync(UserInfoRequest userInfoRequest, CarRentRequest carRentRequest);
}