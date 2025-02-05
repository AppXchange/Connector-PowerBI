namespace Connector.Imports.v1;
using Connector.Imports.v1.Import;
using Connector.Imports.v1.Import.Create;
using Connector.Imports.v1.TempUploadLocation;
using Connector.Imports.v1.TempUploadLocation.Create;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.Action;

public class ImportsV1ActionProcessorServiceDefinition : BaseActionHandlerServiceDefinition<ImportsV1ActionProcessorConfig>
{
    public override string ModuleId => "imports-1";
    public override Type ServiceType => typeof(GenericActionHandlerService<ImportsV1ActionProcessorConfig>);

    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
        var serviceConfig = JsonSerializer.Deserialize<ImportsV1ActionProcessorConfig>(serviceConfigJson, options);
        serviceCollection.AddSingleton<ImportsV1ActionProcessorConfig>(serviceConfig!);
        serviceCollection.AddSingleton<GenericActionHandlerService<ImportsV1ActionProcessorConfig>>();
        serviceCollection.AddSingleton<IActionHandlerServiceDefinition<ImportsV1ActionProcessorConfig>>(this);
        // Register Action Handlers as scoped dependencies
        serviceCollection.AddScoped<CreateImportHandler>();
        serviceCollection.AddScoped<CreateTempUploadLocationHandler>();
    }

    public override void ConfigureService(IActionHandlerService service, ImportsV1ActionProcessorConfig config)
    {
        // Register Action Handler configurations for the Action Processor Service
        service.RegisterHandlerForDataObjectAction<CreateImportHandler, ImportDataObject>(ModuleId, "import", "create", config.CreateImportConfig);
        service.RegisterHandlerForDataObjectAction<CreateTempUploadLocationHandler, TempUploadLocationDataObject>(ModuleId, "temp-upload-location", "create", config.CreateTempUploadLocationConfig);
    }
}