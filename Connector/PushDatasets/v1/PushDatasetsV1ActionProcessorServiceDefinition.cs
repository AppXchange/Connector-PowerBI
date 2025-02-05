namespace Connector.PushDatasets.v1;
using Connector.PushDatasets.v1.Dataset;
using Connector.PushDatasets.v1.Dataset.Create;
using Connector.PushDatasets.v1.Rows;
using Connector.PushDatasets.v1.Rows.Add;
using Connector.PushDatasets.v1.Rows.Delete;
using Connector.PushDatasets.v1.Table;
using Connector.PushDatasets.v1.Table.Update;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.Action;

public class PushDatasetsV1ActionProcessorServiceDefinition : BaseActionHandlerServiceDefinition<PushDatasetsV1ActionProcessorConfig>
{
    public override string ModuleId => "pushdatasets-1";
    public override Type ServiceType => typeof(GenericActionHandlerService<PushDatasetsV1ActionProcessorConfig>);

    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        var serviceConfig = JsonSerializer.Deserialize<PushDatasetsV1ActionProcessorConfig>(serviceConfigJson, options)
            ?? throw new InvalidOperationException("Failed to deserialize service configuration");

        serviceCollection.AddSingleton(serviceConfig);
        serviceCollection.AddSingleton<GenericActionHandlerService<PushDatasetsV1ActionProcessorConfig>>();
        serviceCollection.AddSingleton<IActionHandlerServiceDefinition<PushDatasetsV1ActionProcessorConfig>>(this);

        // Register Action Handlers
        serviceCollection.AddScoped<CreateDatasetHandler>();
        serviceCollection.AddScoped<UpdateTableHandler>();
        serviceCollection.AddScoped<AddRowsHandler>();
        serviceCollection.AddScoped<DeleteRowsHandler>();
    }

    public override void ConfigureService(IActionHandlerService service, PushDatasetsV1ActionProcessorConfig config)
    {
        // Register handlers for each action type
        service.RegisterHandlerForDataObjectAction<CreateDatasetHandler, DatasetDataObject>(
            ModuleId, "dataset", "create", config.CreateDatasetConfig);

        service.RegisterHandlerForDataObjectAction<UpdateTableHandler, TableDataObject>(
            ModuleId, "table", "update", config.UpdateTableConfig);

        service.RegisterHandlerForDataObjectAction<AddRowsHandler, RowsDataObject>(
            ModuleId, "rows", "add", config.AddRowsConfig);

        service.RegisterHandlerForDataObjectAction<DeleteRowsHandler, RowsDataObject>(
            ModuleId, "rows", "delete", config.DeleteRowsConfig);
    }
}