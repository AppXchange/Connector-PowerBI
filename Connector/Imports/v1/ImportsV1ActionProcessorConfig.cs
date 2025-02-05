namespace Connector.Imports.v1;
using Connector.Imports.v1.Import.Create;
using Connector.Imports.v1.TempUploadLocation.Create;
using Json.Schema.Generation;
using Xchange.Connector.SDK.Action;

/// <summary>
/// Configuration for the Action Processor for this module. This configuration will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// The schema will be used for validation at runtime to make sure the configurations are properly formed. 
/// The schema also helps provide integrators more information for what the values are intended to be.
/// </summary>
[Title("Imports V1 Action Processor Configuration")]
[Description("Configuration of the Power BI import actions")]
public class ImportsV1ActionProcessorConfig
{
    [Description("Configuration for creating new imports in Power BI")]
    public DefaultActionHandlerConfig CreateImportConfig { get; set; } = new();

    [Description("Configuration for creating temporary upload locations for large files (1-10 GB)")]
    public DefaultActionHandlerConfig CreateTempUploadLocationConfig { get; set; } = new();
}