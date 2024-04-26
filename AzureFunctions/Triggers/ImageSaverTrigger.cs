using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Triggers;

public class ImageSaverTrigger
{
    private readonly string _containerName;
    private readonly string _connectionString;

    public ImageSaverTrigger(IConfiguration configuration)
    {
        _containerName = configuration["ContainerBlob"]!;
        _connectionString = configuration["BlobConnectionString"]!;
    }
    
    [FunctionName("ImageSaverTrigger")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "save-image")] HttpRequest req, ILogger log)
    {
        if (req.Form.Files.Count == 0)
        {
            return new BadRequestObjectResult("No file was uploaded.");
        }

        foreach (var file in req.Form.Files)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(file.FileName);

            try
            {
                await blobClient.UploadAsync(file.OpenReadStream(), true);
                log.LogInformation("Successfully added file to blob");
            }
            catch (Exception ex)
            {
                log.LogError("Error uploading file: {errorMessage}", ex.Message);
            }
        
            log.LogInformation($"Uploaded file: {file.FileName}, Size: {file.Length} bytes");
        }
        

        return new OkObjectResult($"{req.Form.Files.Count} files were uploaded successfully.");
    }
}