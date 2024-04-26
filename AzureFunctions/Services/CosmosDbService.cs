using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace AzureFunctions.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly IConfiguration _configuration;
    private readonly CosmosClient _client;
    private const string QueryByName = $"SELECT * FROM c WHERE c.name = @name";

    
    public CosmosDbService(IConfiguration configuration)
    {
        _configuration = configuration;
        var account = _configuration["Account"];
        var key = _configuration["CosmosDBKey"];
        _client = new CosmosClient(account, key);
    }

    public async Task AddAsync(ImageInfo item)
    {
        var container = await CreateContainer();
        await container.CreateItemAsync(item, new PartitionKey(item.Id.ToString()));
    }

    public async Task DeleteAsync(Guid id)
    {
        var container = await CreateContainer();
        await container.DeleteItemAsync<ImageInfo>(id.ToString(), new PartitionKey(id.ToString()));
    }

    public async Task<ImageInfo> GetAsync(Guid id)
    {
        try
        {
            var container = await CreateContainer();
            var response = await container.ReadItemAsync<ImageInfo>(id.ToString(), new PartitionKey(id.ToString()));
            return response.Resource;
        }
        catch (CosmosException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<ImageInfo>> GetMultipleAsync(QueryDefinition queryDefinition)
    {
        var container = await CreateContainer();
        var query = container.GetItemQueryIterator<ImageInfo>(queryDefinition);

        var results = new List<ImageInfo>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task UpdateAsync(Guid id, ImageInfo item)
    {
        var container = await CreateContainer();
        await container.UpsertItemAsync(item, new PartitionKey(id.ToString()));
    }
    
    public async Task<IEnumerable<ImageInfo>> GetByNameAsync(string name)
    {
        var container = await CreateContainer();
        // Using the SQL API query syntax to find items with the specified name.
        var queryDefinition = new QueryDefinition(QueryByName).WithParameter("@name", name);

        var query = container.GetItemQueryIterator<ImageInfo>(queryDefinition);

        var results = new List<ImageInfo>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    private async Task<Container> CreateContainer()
    {
        var databaseName = _configuration["DatabaseName"];
        var containerName = _configuration["ContainerCosmosDB"];
        var database = await _client.CreateDatabaseIfNotExistsAsync(databaseName);
        return await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
    }
}