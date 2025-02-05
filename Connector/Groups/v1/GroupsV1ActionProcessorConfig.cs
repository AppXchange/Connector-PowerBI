namespace Connector.Groups.v1;
using Connector.Groups.v1.Group.Create;
using Json.Schema.Generation;
using Xchange.Connector.SDK.Action;

/// <summary>
/// Configuration for the Groups V1 Action Processor module. This configuration defines settings
/// for handling Power BI workspace (group) related actions.
/// </summary>
[Title("Groups V1 Action Processor Configuration")]
[Description("Configuration for Power BI workspace (group) related actions")]
public class GroupsV1ActionProcessorConfig
{
    /// <summary>
    /// Configuration for creating new Power BI workspaces
    /// </summary>
    [Title("Create Group Configuration")]
    [Description("Settings for creating new Power BI workspaces")]
    public DefaultActionHandlerConfig CreateGroupConfig { get; set; } = new();
}