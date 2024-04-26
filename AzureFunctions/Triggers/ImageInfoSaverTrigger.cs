using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctions.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace AzureFunctions.Triggers;

public class ImageInfoSaverTrigger
{
    private readonly ICosmosDbService _cosmosDbService;
    private const string QueryByName = $"SELECT * FROM c WHERE c.name = @name";

    public ImageInfoSaverTrigger(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [FunctionName("ImageInfoSaverTrigger")]
    public async Task RunAsync([BlobTrigger("images/{name}", Connection = "BlobConnectionString")] Stream myBlob,
        string name, ILogger log)
    {
        try
        {
            var image = await Image.LoadAsync(myBlob);
            myBlob.Position = 0;
            using var memoryStream = new MemoryStream();
            await myBlob.CopyToAsync(memoryStream);
            var blobData = memoryStream.ToArray();
            
            var queryDefinition = new QueryDefinition(QueryByName).WithParameter("@name", name);
            var infosByName = (await _cosmosDbService.GetMultipleAsync(queryDefinition)).ToList();

            var imageId = Guid.NewGuid();
                            
            var imageInfo = new ImageInfo()
            {
                Height = image.Height,
                Width = image.Width,
                Size = myBlob.Length,
                Name = name,
                BlobData = blobData,
                AdditionTime = DateTime.UtcNow
            };
            if (!infosByName.Any())
            {
                imageInfo.Id = imageId;
                await _cosmosDbService.AddAsync(imageInfo);
                
                log.LogInformation($"Blob trigger function processed blob\nName: {name}\nSize: {myBlob.Length} Bytes\nDimensions: {image.Width}x{image.Height}\nAdded image info with id: {imageId}");
                return;
            }

            var imageFromDb = infosByName.First();
            imageId = imageFromDb.Id;
            imageInfo.Id = imageId;
            await _cosmosDbService.UpdateAsync(imageId, imageInfo);
                        
            log.LogInformation($"Blob trigger function processed blob\nName: {name}\nSize: {myBlob.Length} Bytes\nDimensions: {image.Width}x{image.Height}\nUpdated image info with id: {imageId}");
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing blob: {ex.Message}");
        }
    }
}