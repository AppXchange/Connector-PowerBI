namespace Connector.PushDatasets.v1.Table.Update;

using Json.Schema.Generation;
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
[Description("Updates the metadata and schema for a table within a Power BI push dataset")]
public class UpdateTableAction : IStandardAction<UpdateTableActionInput, UpdateTableActionOutput>
{
    public UpdateTableActionInput ActionInput { get; set; } = new();
    public UpdateTableActionOutput ActionOutput { get; set; } = new();
    public StandardActionFailure ActionFailure { get; set; } = new();

    public bool CreateRtap => true;
}

public class UpdateTableActionInput
{
    [JsonPropertyName("datasetId")]
    [Description("The ID of the dataset containing the table")]
    public string DatasetId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    [Description("The name of the table to update")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("columns")]
    [Description("The updated column schema for this table")]
    public List<Column> Columns { get; set; } = new();

    [JsonPropertyName("measures")]
    [Description("Optional. The updated measures within this table")]
    public List<Measure>? Measures { get; set; }

    [JsonPropertyName("description")]
    [Description("Optional. The updated table description")]
    public string? Description { get; set; }

    [JsonPropertyName("isHidden")]
    [Description("Optional. Whether this table should be hidden")]
    public bool? IsHidden { get; set; }

    [JsonPropertyName("groupId")]
    [Description("Optional. The workspace/group ID if the dataset is not in My Workspace")]
    public string? GroupId { get; set; }
}

public class UpdateTableActionOutput
{
    [JsonPropertyName("name")]
    [Description("The name of the updated table")]
    public string Name { get; set; } = string.Empty;
}
