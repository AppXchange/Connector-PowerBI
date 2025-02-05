namespace Connector.Imports.v1.TempUploadLocation;

using Json.Schema.Generation;
using System;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.CacheWriter;

/// <summary>
/// Data object that will represent an object in the Xchange system. This will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// These types will be used for validation at runtime to make sure the objects being passed through the system 
/// are properly formed. The schema also helps provide integrators more information for what the values 
/// are intended to be.
/// </summary>
[PrimaryKey("url", nameof(Url))]
[Description("Represents a temporary blob storage upload location for importing large Power BI .pbix files")]
public class TempUploadLocationDataObject
{
    [JsonPropertyName("expirationTime")]
    [Description("The expiration date and time of the shared access signature URL")]
    public required string ExpirationTime { get; init; }

    [JsonPropertyName("url")]
    [Description("The shared access signature URL for the temporary blob storage")]
    public required string Url { get; init; }
}