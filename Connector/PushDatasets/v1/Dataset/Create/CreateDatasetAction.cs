namespace Connector.PushDatasets.v1.Dataset.Create;

using Json.Schema.Generation;
using System;
using System.Collections.Generic;
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
[Description("Creates a new Push dataset in Power BI")]
public class CreateDatasetAction : IStandardAction<CreateDatasetActionInput, CreateDatasetActionOutput>
{
    public CreateDatasetActionInput ActionInput { get; set; } = new();
    public CreateDatasetActionOutput ActionOutput { get; set; } = new();
    public StandardActionFailure ActionFailure { get; set; } = new();

    public bool CreateRtap => true;
}

public class CreateDatasetActionInput
{
    [JsonPropertyName("name")]
    [Description("The name of the dataset")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("defaultMode")]
    [Description("The dataset mode (defaults to Push)")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DatasetMode DefaultMode { get; set; } = DatasetMode.Push;

    [JsonPropertyName("tables")]
    [Description("The tables within the dataset")]
    public List<Table> Tables { get; set; } = new();

    [JsonPropertyName("groupId")]
    [Description("Optional. The workspace/group ID to create the dataset in. If not provided, creates in My Workspace.")]
    public string? GroupId { get; set; }

    [JsonPropertyName("defaultRetentionPolicy")]
    [Description("Optional. The default retention policy for the dataset")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DefaultRetentionPolicy? DefaultRetentionPolicy { get; set; }
}

public class CreateDatasetActionOutput
{
    [JsonPropertyName("id")]
    [Description("The unique identifier of the created dataset")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    [Description("The name of the created dataset")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("defaultRetentionPolicy")]
    [Description("The retention policy of the created dataset")]
    public string? DefaultRetentionPolicy { get; set; }
}
