namespace Connector.Datasets.v1;
using Connector.Datasets.v1.Dataset.Update;
using Json.Schema.Generation;
using Xchange.Connector.SDK.Action;

/// <summary>
/// Configuration for the Action Processor for Power BI Datasets module.
/// </summary>
[Title("Datasets V1 Action Processor Configuration")]
[Description("Configuration for updating Power BI datasets")]
public class DatasetsV1ActionProcessorConfig
{
    [Description("Configuration for updating dataset properties (storage mode and query scale-out settings)")]
    public DefaultActionHandlerConfig UpdateDatasetConfig { get; set; } = new();
}