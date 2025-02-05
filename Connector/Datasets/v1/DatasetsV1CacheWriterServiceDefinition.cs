namespace Connector.Datasets.v1;
using Connector.Datasets.v1.Dataset;
using Connector.Datasets.v1.Datasets;
using Connector.Client;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using Xchange.Connector.SDK.Abstraction.Change;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Hosting.Configuration;

public class DatasetsV1CacheWriterServiceDefinition : BaseCacheWriterServiceDefinition<DatasetsV1CacheWriterConfig>
{
    public override string ModuleId => "datasets-1";
    public override Type ServiceType => typeof(GenericCacheWriterService<DatasetsV1CacheWriterConfig>);

    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var serviceConfig = JsonSerializer.Deserialize<DatasetsV1CacheWriterConfig>(serviceConfigJson);
        
        // Register configurations
        serviceCollection.AddSingleton<DatasetsV1CacheWriterConfig>(serviceConfig!);
        serviceCollection.AddSingleton<GenericCacheWriterService<DatasetsV1CacheWriterConfig>>();
        serviceCollection.AddSingleton<ICacheWriterServiceDefinition<DatasetsV1CacheWriterConfig>>(this);

        // Register data readers for both individual datasets and dataset lists
        serviceCollection.AddSingleton<DatasetDataReader>();
        serviceCollection.AddSingleton<DatasetsDataReader>();

        // Register Power BI client
        serviceCollection.AddHttpClient<IPowerBIClient, ApiClient>()
            .ConfigureHttpClient(client => 
                client.BaseAddress = new Uri("https://api.powerbi.com/v1.0/myorg/"));
    }

    public override IDataObjectChangeDetectorProvider ConfigureChangeDetectorProvider(
        IChangeDetectorFactory factory, 
        ConnectorDefinition connectorDefinition)
    {
        var options = factory.CreateProviderOptionsWithNoDefaultResolver();

        // Configure keys for both dataset types
        this.RegisterKeysForObject<DatasetDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<DatasetsDataObject>(options, connectorDefinition);

        return factory.CreateProvider(options);
    }

    public override void ConfigureService(ICacheWriterService service, DatasetsV1CacheWriterConfig config)
    {
        var dataReaderSettings = new DataReaderSettings
        {
            DisableDeletes = false,
            UseChangeDetection = true
        };

        // Register both dataset readers with their configurations
        service.RegisterDataReader<DatasetDataReader, DatasetDataObject>(
            ModuleId, 
            config.DatasetConfig, 
            dataReaderSettings);

        service.RegisterDataReader<DatasetsDataReader, DatasetsDataObject>(
            ModuleId, 
            config.DatasetsConfig, 
            dataReaderSettings);
    }
}