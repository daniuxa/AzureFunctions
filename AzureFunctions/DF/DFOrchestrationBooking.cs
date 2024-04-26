using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Requests;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.DF;

public class DfOrchestrationBooking
{
    [FunctionName("DFOrchestrationBooking_HttpStart")]
    public async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        if (req.Content is null)
        {
            return null;
        }
        
        var requestBody = await req.Content.ReadAsStringAsync();
        
        var instanceId = 
            await starter.StartNewAsync<string>("DFOrchestrationBooking", requestBody);

        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        return starter.CreateCheckStatusResponse(req, instanceId);
    }

    [FunctionName("DfOrchestrationBooking")]
    public async Task<Dictionary<string, Guid>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var requestBody = context.GetInput<string>();
        var bookingRequest = JsonConvert.DeserializeObject<BookingRequest>(requestBody);
        
        var outputs = new Dictionary<string, Guid>();

        // Flight reservation
        var flightReservationId = 
            await context.CallActivityAsync<Guid>("DFOrchestrationBooking_Flight", 
                (bookingRequest.UserInfoRequest, bookingRequest.FlightBookingRequest));
        if (flightReservationId == Guid.Empty)
        {
            context.SetCustomStatus("Flight was canceled");
            return outputs;
        }
        outputs.Add("Flight", flightReservationId);
        context.SetCustomStatus("Flight was reserved");
        
        
        // Hotel reservation
        var hotelReservationId = 
            await context.CallActivityAsync<Guid>("DFOrchestrationBooking_Hotel", 
                (bookingRequest.UserInfoRequest, bookingRequest.HotelBookingRequest));
        if (hotelReservationId == Guid.Empty)
        {
            context.SetCustomStatus("Hotel was canceled");
            if (bookingRequest.AllowCancelFlight)
            {
                await context.CallActivityAsync<Guid>("DFOrchestrationRollingBack_Flight", 
                    flightReservationId);
                context.SetCustomStatus("Hotel and flight was canceled");
            }
            return outputs;
        }
        outputs.Add("Hotel", hotelReservationId);
        context.SetCustomStatus("Hotel was reserved");
        
        
        // Car reservation
        var carReservationId = 
            await context.CallActivityAsync<Guid>("DFOrchestrationBooking_Car", 
                (bookingRequest.UserInfoRequest, bookingRequest.CarRentRequest));
        if (carReservationId == Guid.Empty)
        {
            context.SetCustomStatus("Car was canceled");
            if (bookingRequest.AllowCancelHotel)
            {
                await context.CallActivityAsync<Guid>("DFOrchestrationRollingBack_Hotel", 
                    carReservationId);
                context.SetCustomStatus("Car and hotel were canceled");
            }
            return outputs;
        }
        outputs.Add("Car", carReservationId);
        context.SetCustomStatus("Car was reserved");
        
        return outputs;
    }
}