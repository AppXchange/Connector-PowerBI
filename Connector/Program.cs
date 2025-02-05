using System;
using ESR.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xchange.Connector.SDK.Hosting;
using Xchange.Connector.SDK.Test.Local;

namespace Connector;

/// <summary>
/// Main executable entry point for the connector.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        // DO NOT MODIFY: This check is to make sure that the connector has the proper environment variable available to it to function.
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ESR__QueueName")))
        {
            Console.Error.WriteLine("Exiting 'ESR__QueueName' environment variable not provided");
            Environment.Exit(1);
        }
        
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLocalDevelopment(args)
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                
                // Set minimum log level from configuration or default to Information
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureServices((context, services) =>
            {
                // Add any additional services here
                services.AddHttpClient();
            })
            .UseGenericServiceRun<ServiceRunner>(new ConnectorRegistration(), args)
            .Build();

        host.Run();
    }
}