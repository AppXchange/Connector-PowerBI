namespace Connector.PushDatasets.v1.Rows.Delete;

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
[Description("Deletes all rows from a Power BI push dataset table")]
public class DeleteRowsAction : IStandardAction<DeleteRowsActionInput, DeleteRowsActionOutput>
{
    public DeleteRowsActionInput ActionInput { get; set; } = new();
    public DeleteRowsActionOutput ActionOutput { get; set; } = new();
    public StandardActionFailure ActionFailure { get; set; } = new();

    public bool CreateRtap => true;
}

public class DeleteRowsActionInput
{
    [JsonPropertyName("datasetId")]
    [Description("The ID of the dataset to delete rows from")]
    public string DatasetId { get; set; } = string.Empty;

    [JsonPropertyName("tableName")]
    [Description("The name of the table to delete rows from")]
    public string TableName { get; set; } = string.Empty;

    [JsonPropertyName("groupId")]
    [Description("Optional. The workspace/group ID if the dataset is not in My Workspace")]
    public string? GroupId { get; set; }
}

public class DeleteRowsActionOutput
{
    [JsonPropertyName("success")]
    [Description("Indicates if the rows were successfully deleted")]
    public bool Success { get; set; }
}
