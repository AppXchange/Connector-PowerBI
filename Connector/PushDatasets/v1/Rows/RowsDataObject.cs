namespace Connector.PushDatasets.v1.Rows;

using Json.Schema.Generation;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.CacheWriter;

/// <summary>
/// Data object that will represent an object in the Xchange system. This will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// These types will be used for validation at runtime to make sure the objects being passed through the system 
/// are properly formed. The schema also helps provide integrators more information for what the values 
/// are intended to be.
/// </summary>
[PrimaryKey("id", nameof(Id))]
//[AlternateKey("alt-key-id", nameof(CompanyId), nameof(EquipmentNumber))]
[Description("Represents rows of data to be pushed to a Power BI dataset table")]
public class RowsDataObject
{
    [JsonPropertyName("id")]
    [Description("The unique identifier for this batch of rows")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("datasetId")]
    [Description("The ID of the dataset to push rows to")]
    public string DatasetId { get; init; } = string.Empty;

    [JsonPropertyName("tableName")]
    [Description("The name of the table to push rows to")]
    public string TableName { get; init; } = string.Empty;

    [JsonPropertyName("groupId")]
    [Description("Optional. The workspace/group ID if the dataset is not in My Workspace")]
    public string? GroupId { get; init; }

    [JsonPropertyName("rows")]
    [Description("The array of data rows to push to the dataset table")]
    public List<Dictionary<string, object>> Rows { get; init; } = new();
}