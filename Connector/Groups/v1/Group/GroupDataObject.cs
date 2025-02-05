namespace Connector.Groups.v1.Group;

using Json.Schema.Generation;
using System;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.CacheWriter;

/// <summary>
/// Represents a Power BI workspace (group) with its properties and settings
/// </summary>
[PrimaryKey("id", nameof(Id))]
[Description("A Power BI workspace (group) containing reports, dashboards, and datasets")]
public class GroupDataObject
{
    [JsonPropertyName("id")]
    [Description("The workspace ID")]
    public Guid Id { get; init; }

    [JsonPropertyName("name")]
    [Description("The workspace name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("isReadOnly")]
    [Description("Whether the workspace is read-only")]
    public bool IsReadOnly { get; init; }

    [JsonPropertyName("isOnDedicatedCapacity")]
    [Description("Whether the workspace is assigned to a dedicated capacity")]
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

    [JsonPropertyName("logAnalyticsWorkspace")]
    [Description("The Log Analytics workspace assigned to the group")]
    public AzureResource? LogAnalyticsWorkspace { get; init; }
}

/// <summary>
/// Represents an Azure resource such as a Log Analytics workspace
/// </summary>
public class AzureResource
{
    [JsonPropertyName("id")]
    [Description("An identifier for the resource within Power BI")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("resourceGroup")]
    [Description("The resource group within the subscription where the resource resides")]
    public string ResourceGroup { get; init; } = string.Empty;

    [JsonPropertyName("resourceName")]
    [Description("The name of the resource")]
    public string ResourceName { get; init; } = string.Empty;

    [JsonPropertyName("subscriptionId")]
    [Description("The Azure subscription where the resource resides")]
    public string SubscriptionId { get; init; } = string.Empty;
}