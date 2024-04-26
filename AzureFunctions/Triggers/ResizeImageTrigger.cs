using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using AzureFunctions.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AzureFunctions.Triggers;

public class ResizeImageTrigger
{
    private readonly ICosmosDbService _cosmosDbService;
    private readonly string _containerName;
    private readonly string _connectionString;
    private const string QueryById = $"SELECT * FROM c WHERE c.id = @id";

    public ResizeImageTrigger(IConfiguration configuration, ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
        _containerName = configuration["ContainerBlob"]!;
        _connectionString = configuration["BlobConnectionString"]!;
    }
    
    [FunctionName("ResizeImageTrigger")]
    public async Task RunAsync([CosmosDBTrigger(
            databaseName: "Images",
            containerName: "Info",
            Connection = "AzureDBConnectionString",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]
        IReadOnlyList<ImageInfo> input,
        ILogger log)
    {
        if (!input.Any())
        {
            return;
        }
        
        foreach (var imageInfo in input)
        {
            var id = imageInfo.Id;
            var queryDefinition = new QueryDefinition(QueryById).WithParameter("@id", id);
            var infosById = (await _cosmosDbService.GetMultipleAsync(queryDefinition)).ToList();
            if (infosById.Any() && infosById.First().Height == 100 && infosById.First().Width == 100)
            {
                log.LogInformation("Duplicated image info");
                continue;
            }
            
            using var stream = new MemoryStream(imageInfo.BlobData);
            var image = await Image.LoadAsync(stream);
            image.Mutate(x => x.Resize(new Size(100, 100)));

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(imageInfo.Name);
            var streamOfConvertedImage = new MemoryStream();
                
            if (image.Metadata.DecodedImageFormat != null)
                await image.SaveAsync(streamOfConvertedImage, image.Metadata.DecodedImageFormat);
            streamOfConvertedImage.Position = 0;
            try
            {
                await blobClient.UploadAsync(streamOfConvertedImage, true);
                log.LogInformation("Successfully added file to blob");
            }
            catch (Exception ex)
            {
                log.LogError("Error uploading file: {errorMessage}", ex.Message);
            }
            
        }
        
        log.LogInformation("Documents modified " + input.Count);
    }
}