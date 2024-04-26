using System;
using System.Threading.Tasks;

namespace AzureFunctions.Services;

public interface IRollbackBooking
{
    Task RollbackBooking(Guid reservationId);
}