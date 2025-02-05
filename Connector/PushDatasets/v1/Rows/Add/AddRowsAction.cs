namespace Connector.PushDatasets.v1.Rows.Add;

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
[Description("Adds new rows to a Power BI push dataset table")]
public class AddRowsAction : IStandardAction<AddRowsActionInput, AddRowsActionOutput>
{
    public AddRowsActionInput ActionInput { get; set; } = new();
    public AddRowsActionOutput ActionOutput { get; set; } = new();
    public StandardActionFailure ActionFailure { get; set; } = new();

    public bool CreateRtap => true;
}

public class AddRowsActionInput
{
    [JsonPropertyName("datasetId")]
    [Description("The ID of the dataset to push rows to")]
    public string DatasetId { get; set; } = string.Empty;

    [JsonPropertyName("tableName")]
    [Description("The name of the table to push rows to")]
    public string TableName { get; set; } = string.Empty;

    [JsonPropertyName("groupId")]
    [Description("Optional. The workspace/group ID if the dataset is not in My Workspace")]
    public string? GroupId { get; set; }

    [JsonPropertyName("rows")]
    [Description("The array of data rows to push to the dataset table")]
    public List<Dictionary<string, object>> Rows { get; set; } = new();
}

public class AddRowsActionOutput
{
    [JsonPropertyName("success")]
    [Description("Indicates if the rows were successfully added")]
    public bool Success { get; set; }

    [JsonPropertyName("rowCount")]
    [Description("Number of rows that were added")]
    public int RowCount { get; set; }
}
