namespace Connector.PushDatasets.v1;
using Connector.PushDatasets.v1.Dataset;
using Connector.PushDatasets.v1.Tables;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using Xchange.Connector.SDK.Abstraction.Change;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Hosting.Configuration;
using Connector.PushDatasets.v1.Rows;
using Connector.PushDatasets.v1.Table;

public class PushDatasetsV1CacheWriterServiceDefinition : BaseCacheWriterServiceDefinition<PushDatasetsV1CacheWriterConfig>
{
    public override string ModuleId => "pushdatasets-1";
    public override Type ServiceType => typeof(GenericCacheWriterService<PushDatasetsV1CacheWriterConfig>);

    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var serviceConfig = JsonSerializer.Deserialize<PushDatasetsV1CacheWriterConfig>(serviceConfigJson);
        serviceCollection.AddSingleton<PushDatasetsV1CacheWriterConfig>(serviceConfig!);
        serviceCollection.AddSingleton<GenericCacheWriterService<PushDatasetsV1CacheWriterConfig>>();
        serviceCollection.AddSingleton<ICacheWriterServiceDefinition<PushDatasetsV1CacheWriterConfig>>(this);
        // Register Data Readers as Singletons
        serviceCollection.AddSingleton<TablesDataReader>();
        serviceCollection.AddSingleton<DatasetDataReader>();
    }

    public override IDataObjectChangeDetectorProvider ConfigureChangeDetectorProvider(IChangeDetectorFactory factory, ConnectorDefinition connectorDefinition)
    {
        var options = factory.CreateProviderOptionsWithNoDefaultResolver();
        
        // Configure Data Object Keys for Data Objects
        this.RegisterKeysForObject<TablesDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<DatasetDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<TableDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<RowsDataObject>(options, connectorDefinition);

        return factory.CreateProvider(options);
    }

    public override void ConfigureService(ICacheWriterService service, PushDatasetsV1CacheWriterConfig config)
    {
        var dataReaderSettings = new DataReaderSettings
        {
            DisableDeletes = false,
            UseChangeDetection = true
        };

        // Register Data Reader configurations for the Cache Writer Service
        service.RegisterIncrementalDataReader<TablesDataReader, TablesDataObject>(ModuleId, config.TablesConfig, dataReaderSettings);
        service.RegisterDataReader<DatasetDataReader, DatasetDataObject>(ModuleId, config.DatasetConfig, dataReaderSettings);
    }
}