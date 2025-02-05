namespace Connector.Groups.v1;
using Connector.Groups.v1.Group;
using Connector.Groups.v1.Groups;
using ESR.Hosting.CacheWriter;
using Json.Schema.Generation;

/// <summary>
/// Configuration for the Groups V1 Cache Writer module. This configuration defines settings
/// for caching Power BI workspace (group) data.
/// </summary>
[Title("Groups V1 Cache Writer Configuration")]
[Description("Configuration for Power BI workspace data caching")]
public class GroupsV1CacheWriterConfig
{
    /// <summary>
    /// Configuration for caching detailed Power BI workspace data
    /// </summary>
    [Title("Group Cache Configuration")]
    [Description("Settings for caching detailed Power BI workspace data")]
    public CacheWriterObjectConfig GroupConfig { get; set; } = new();

    /// <summary>
    /// Configuration for caching basic Power BI workspace list data
    /// </summary>
    [Title("Groups List Cache Configuration")]
    [Description("Settings for caching basic Power BI workspace list data")]
    public CacheWriterObjectConfig GroupsConfig { get; set; } = new();
}