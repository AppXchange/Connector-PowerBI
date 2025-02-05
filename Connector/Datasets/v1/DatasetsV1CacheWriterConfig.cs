namespace Connector.Datasets.v1;
using Connector.Datasets.v1.Dataset;
using Connector.Datasets.v1.Datasets;
using ESR.Hosting.CacheWriter;
using Json.Schema.Generation;

/// <summary>
/// Configuration for Power BI Datasets cache writer.
/// </summary>
[Title("Datasets V1 Cache Writer Configuration")]
[Description("Configuration for reading and caching Power BI datasets")]
public class DatasetsV1CacheWriterConfig
{
    [Description("Configuration for reading individual datasets")]
    public CacheWriterObjectConfig DatasetConfig { get; set; } = new();

    [Description("Configuration for reading lists of datasets")]
    public CacheWriterObjectConfig DatasetsConfig { get; set; } = new();
}