namespace Connector.Datasets.v1;
using Connector.Datasets.v1.Dataset;
using Connector.Datasets.v1.Dataset.Update;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.Action;

public class DatasetsV1ActionProcessorServiceDefinition : BaseActionHandlerServiceDefinition<DatasetsV1ActionProcessorConfig>
{
    public override string ModuleId => "datasets-1";
    public override Type ServiceType => typeof(GenericActionHandlerService<DatasetsV1ActionProcessorConfig>);

    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
        var serviceConfig = JsonSerializer.Deserialize<DatasetsV1ActionProcessorConfig>(serviceConfigJson, options);
        serviceCollection.AddSingleton<DatasetsV1ActionProcessorConfig>(serviceConfig!);
        serviceCollection.AddSingleton<GenericActionHandlerService<DatasetsV1ActionProcessorConfig>>();
        serviceCollection.AddSingleton<IActionHandlerServiceDefinition<DatasetsV1ActionProcessorConfig>>(this);

        // Register the update dataset handler
        serviceCollection.AddScoped<UpdateDatasetHandler>();
    }

    public override void ConfigureService(IActionHandlerService service, DatasetsV1ActionProcessorConfig config)
    {
        // Register the update dataset action handler
        service.RegisterHandlerForDataObjectAction<UpdateDatasetHandler, DatasetDataObject>(
            ModuleId, 
            "dataset", 
            "update", 
            config.UpdateDatasetConfig);
    }
}