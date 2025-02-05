namespace Connector.Datasets.v1.Dataset.Update;

using Json.Schema.Generation;
using System;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Action;

/// <summary>
/// Action object that will represent an action in the Xchange system. This will contain an input object type,
/// an output object type, and a Action failure type (this will default to <see cref="StandardActionFailure"/>
/// but that can be overridden with your own preferred type). These objects will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// These types will be used for validation at runtime to make sure the objects being passed through the system 
/// are properly formed. The schema also helps provide integrators more information for what the values 
/// are intended to be.
/// </summary>
[Description("Updates a Power BI dataset's properties like storage mode and query scale-out settings")]
public class UpdateDatasetAction : IStandardAction<UpdateDatasetActionInput, UpdateDatasetActionOutput>
{
    public UpdateDatasetActionInput ActionInput { get; set; } = new() { Id = string.Empty };
    public UpdateDatasetActionOutput ActionOutput { get; set; } = new() { Id = string.Empty };
    public StandardActionFailure ActionFailure { get; set; } = new();

    public bool CreateRtap => true;
}

public class UpdateDatasetActionInput
{
    [JsonPropertyName("id")]
    [Description("The dataset ID to update")]
    public required string Id { get; init; }

    [JsonPropertyName("groupId")]
    [Description("Optional workspace/group ID. If not provided, updates dataset in My workspace")]
    public string? GroupId { get; init; }

    [JsonPropertyName("targetStorageMode")]
    [Description("The dataset storage mode (e.g. 'PremiumFiles' or 'Abf')")]
    public string? TargetStorageMode { get; init; }

    [JsonPropertyName("queryScaleOutSettings")]
    [Description("Query scale-out settings of a dataset")]
    public QueryScaleOutSettings? QueryScaleOutSettings { get; init; }
}

public class QueryScaleOutSettings
{
    [JsonPropertyName("autoSyncReadOnlyReplicas")]
    [Description("Whether the dataset automatically syncs read-only replicas")]
    public bool AutoSyncReadOnlyReplicas { get; init; }

    [JsonPropertyName("maxReadOnlyReplicas")]
    [Description("Maximum number of read-only replicas for the dataset (0-64, -1 for automatic number of replicas)")]
    public int MaxReadOnlyReplicas { get; init; }
}

public class UpdateDatasetActionOutput
{
    [JsonPropertyName("id")]
    [Description("The dataset ID that was updated")]
    public required string Id { get; init; }
}
