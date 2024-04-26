using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace AzureFunctions.Services;

public interface ICosmosDbService
{
    Task<IEnumerable<ImageInfo>> GetMultipleAsync(QueryDefinition queryDefinition);
    Task<ImageInfo> GetAsync(Guid id);
    Task AddAsync(ImageInfo item);
    Task UpdateAsync(Guid id, ImageInfo item);
    Task DeleteAsync(Guid id);
}