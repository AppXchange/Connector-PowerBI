namespace Connector.Imports.v1.TempUploadLocation.Create;

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
[Description("Creates a temporary blob storage upload location for importing large Power BI .pbix files (1-10 GB)")]
public class CreateTempUploadLocationAction : IStandardAction<CreateTempUploadLocationActionInput, CreateTempUploadLocationActionOutput>
{
    public CreateTempUploadLocationActionInput ActionInput { get; set; } = new();
    public CreateTempUploadLocationActionOutput ActionOutput { get; set; } = new()
    {
        ExpirationTime = string.Empty,
        Url = string.Empty
    };
    public StandardActionFailure ActionFailure { get; set; } = new();
    public bool CreateRtap => true;
}

public class CreateTempUploadLocationActionInput
{
    [JsonPropertyName("groupId")]
    [Description("Optional. The workspace ID. If not provided, creates upload location in My Workspace")]
    public string? GroupId { get; set; }
}

public class CreateTempUploadLocationActionOutput
{
    [JsonPropertyName("expirationTime")]
    [Description("The expiration date and time of the shared access signature URL")]
    public required string ExpirationTime { get; init; }

    [JsonPropertyName("url")]
    [Description("The shared access signature URL for the temporary blob storage")]
    public required string Url { get; init; }
}
