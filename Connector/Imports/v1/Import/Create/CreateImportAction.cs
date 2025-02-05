namespace Connector.Imports.v1.Import.Create;

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
[Description("Creates a new import in Power BI, either in My Workspace or a specified group")]
public class CreateImportAction : IStandardAction<CreateImportActionInput, CreateImportActionOutput>
{
    public CreateImportActionInput ActionInput { get; set; } = new()
    {
        DatasetDisplayName = string.Empty,
        FileContent = string.Empty
    };
    public CreateImportActionOutput ActionOutput { get; set; } = new()
    {
        Id = string.Empty,
        ImportState = string.Empty,
        CreatedDateTime = string.Empty,
        UpdatedDateTime = string.Empty,
        Name = string.Empty
    };
    public StandardActionFailure ActionFailure { get; set; } = new();
    public bool CreateRtap => true;
}

public class CreateImportActionInput
{
    [JsonPropertyName("groupId")]
    [Description("Optional. The workspace/group ID to import into. If not provided, imports into My Workspace")]
    public string? GroupId { get; set; }

    [JsonPropertyName("datasetDisplayName")]
    [Description("The display name of the dataset, should include file extension")]
    public required string DatasetDisplayName { get; set; }

    [JsonPropertyName("fileContent")]
    [Description("The base64 encoded content of the file to import")]
    public required string FileContent { get; set; }

    [JsonPropertyName("nameConflict")]
    [Description("Specifies what to do if a dataset with the same name exists")]
    public ImportConflictHandlerMode? NameConflict { get; set; }

    [JsonPropertyName("skipReport")]
    [Description("Whether to skip report import. Only supported for Power BI .pbix files")]
    public bool? SkipReport { get; set; }

    [JsonPropertyName("overrideReportLabel")]
    [Description("Whether to override existing report label when republishing")]
    public bool? OverrideReportLabel { get; set; }

    [JsonPropertyName("overrideModelLabel")]
    [Description("Whether to override existing model label when republishing")]
    public bool? OverrideModelLabel { get; set; }
}

public class CreateImportActionOutput
{
    [JsonPropertyName("id")]
    [Description("The import ID")]
    public required string Id { get; set; }

    [JsonPropertyName("importState")]
    [Description("The import upload state")]
    public required string ImportState { get; set; }

    [JsonPropertyName("createdDateTime")]
    [Description("Import creation date and time")]
    public required string CreatedDateTime { get; set; }

    [JsonPropertyName("updatedDateTime")]
    [Description("Import last update date and time")]
    public required string UpdatedDateTime { get; set; }

    [JsonPropertyName("name")]
    [Description("The import name")]
    public required string Name { get; set; }
}

public enum ImportConflictHandlerMode
{
    Abort,
    CreateOrOverwrite,
    GenerateUniqueName,
    Ignore,
    Overwrite
}
