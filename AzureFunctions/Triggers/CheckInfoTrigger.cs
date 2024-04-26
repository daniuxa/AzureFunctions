using System;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctions.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Triggers;

public class CheckInfoTrigger
{
    private readonly ICosmosDbService _cosmosDbService;
    private const string QueryTemplate = "SELECT * FROM c WHERE c.additionTime >= '@timeFrom'";

    public CheckInfoTrigger(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }
    [FunctionName("CheckInfoTrigger")]
    public async Task RunAsync([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
    {
        var dateTimeNow = DateTime.UtcNow;
        var dateTimeFrom = DateTime.UtcNow.AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        var queryDefinition = new QueryDefinition(QueryTemplate).WithParameter("@timeFrom", dateTimeFrom);
        var imageInfos = (await _cosmosDbService.GetMultipleAsync(queryDefinition)).ToList();
        
        log.LogInformation($"From {dateTimeFrom} to {dateTimeNow} were added {imageInfos.Count} count of images:");
        foreach (var imageInfo in imageInfos)
        {
            log.LogInformation($"Name: {imageInfo.Name}, Height: {imageInfo.Height}, Width: {imageInfo.Width}, Size: {imageInfo.Size}");
        }
    }
}