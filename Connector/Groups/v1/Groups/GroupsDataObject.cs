namespace Connector.Groups.v1.Groups;

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
[PrimaryKey("id", nameof(Id))]
//[AlternateKey("alt-key-id", nameof(CompanyId), nameof(EquipmentNumber))]
[Description("Represents a Power BI workspace (group) that a user has access to.")]
public class GroupsDataObject
{
    [JsonPropertyName("id")]
    [Description("The workspace ID")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    [Description("The group name")]
    public required string Name { get; init; }

    [JsonPropertyName("isReadOnly")]
    [Description("Whether the group is read-only")]
    public bool IsReadOnly { get; init; }

    [JsonPropertyName("isOnDedicatedCapacity")]
    [Description("Whether the group is assigned to a dedicated capacity")]
    public bool IsOnDedicatedCapacity { get; init; }

    [JsonPropertyName("capacityId")]
    [Description("The capacity ID")]
    public string? CapacityId { get; init; }

    [JsonPropertyName("defaultDatasetStorageFormat")]
    [Description("The default dataset storage format in the workspace. Only returned when isOnDedicatedCapacity is true")]
    public string? DefaultDatasetStorageFormat { get; init; }

    [JsonPropertyName("dataflowStorageId")]
    [Description("The Power BI dataflow storage account ID")]
    public string? DataflowStorageId { get; init; }
}