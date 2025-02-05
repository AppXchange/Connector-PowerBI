namespace Connector.PushDatasets.v1;
using Connector.PushDatasets.v1.Dataset.Create;
using Connector.PushDatasets.v1.Rows.Add;
using Connector.PushDatasets.v1.Rows.Delete;
using Connector.PushDatasets.v1.Table.Update;
using Json.Schema.Generation;
using Xchange.Connector.SDK.Action;

/// <summary>
/// Configuration for the Action Processor for this module. This configuration will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// The schema will be used for validation at runtime to make sure the configurations are properly formed. 
/// The schema also helps provide integrators more information for what the values are intended to be.
/// </summary>
[Title("PushDatasets V1 Action Processor Configuration")]
[Description("Configuration of the Power BI Push Dataset actions")]
public class PushDatasetsV1ActionProcessorConfig
{
    [Description("Configuration for creating datasets")]
    public DefaultActionHandlerConfig CreateDatasetConfig { get; set; } = new();

    [Description("Configuration for updating table schemas")]
    public DefaultActionHandlerConfig UpdateTableConfig { get; set; } = new();

    [Description("Configuration for adding rows to tables")]
    public DefaultActionHandlerConfig AddRowsConfig { get; set; } = new();

    [Description("Configuration for deleting rows from tables")]
    public DefaultActionHandlerConfig DeleteRowsConfig { get; set; } = new();

    [Description("Optional. The workspace/group ID if not using My Workspace")]
    public string? GroupId { get; set; }
}