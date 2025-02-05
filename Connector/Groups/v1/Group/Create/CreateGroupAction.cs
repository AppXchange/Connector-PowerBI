namespace Connector.Groups.v1.Group.Create;

using Json.Schema.Generation;
using System;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Action;

/// <summary>
/// Action to create a new Power BI workspace (group)
/// </summary>
[Description("Creates a new Power BI workspace")]
public class CreateGroupAction : IStandardAction<CreateGroupActionInput, CreateGroupActionOutput>
{
    public CreateGroupActionInput ActionInput { get; set; } = new();
    public CreateGroupActionOutput ActionOutput { get; set; } = new();
    public StandardActionFailure ActionFailure { get; set; } = new();

    public bool CreateRtap => true;
}

/// <summary>
/// Input parameters for creating a new workspace
/// </summary>
public class CreateGroupActionInput
{
    [JsonPropertyName("name")]
    [Required]
    [Description("The name of the workspace to create")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("workspaceV2")]
    [Description("Whether to create a V2 workspace. The only supported value is true.")]
    public bool? WorkspaceV2 { get; set; }
}

/// <summary>
/// Response data from creating a new workspace
/// </summary>
public class CreateGroupActionOutput
{
    [JsonPropertyName("id")]
    [Description("The ID of the created workspace")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    [Description("The name of the created workspace")] 
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("isReadOnly")]
    [Description("Whether the workspace is read-only")]
    public bool IsReadOnly { get; set; }

    [JsonPropertyName("isOnDedicatedCapacity")]
    [Description("Whether the workspace is on dedicated capacity")]
    public bool IsOnDedicatedCapacity { get; set; }
}
