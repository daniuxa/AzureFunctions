using System;
using Newtonsoft.Json;

namespace AzureFunctions;

public class ImageInfo
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }
    
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "size")]
    public long Size { get; set; }
    
    [JsonProperty(PropertyName = "width")]
    public int Width { get; set; }
    
    [JsonProperty(PropertyName = "height")]
    public int Height { get; set; }
    
    [JsonProperty(PropertyName = "blobData")]
    public byte[] BlobData { get; set; }
    
    [JsonProperty(PropertyName = "additionTime")]
    public DateTime AdditionTime { get; set; }
}