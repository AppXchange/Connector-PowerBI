namespace Connector.PushDatasets.v1;
using Connector.PushDatasets.v1.Dataset;
using Connector.PushDatasets.v1.Tables;
using ESR.Hosting.CacheWriter;
using Json.Schema.Generation;

/// <summary>
/// Configuration for the Cache writer for this module. This configuration will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// The schema will be used for validation at runtime to make sure the configurations are properly formed. 
/// The schema also helps provide integrators more information for what the values are intended to be.
/// </summary>
[Title("PushDatasets V1 Cache Writer Configuration")]
[Description("Configuration of the data object caches for Power BI Push Datasets.")]
public class PushDatasetsV1CacheWriterConfig
{
    [Description("Configuration for reading Power BI tables")]
    public IncrementalCacheWriterConfig TablesConfig { get; set; } = new();

    [Description("Configuration for reading Power BI datasets")]
    public CacheWriterObjectConfig DatasetConfig { get; set; } = new();

    [Description("Configuration for reading Power BI rows")]
    public CacheWriterObjectConfig RowsConfig { get; set; } = new();

    [Description("Optional. The workspace/group ID if not using My Workspace")]
    public string? GroupId { get; set; }

    [Description("The dataset ID to read from")]
    public string DatasetId { get; set; } = string.Empty;
}