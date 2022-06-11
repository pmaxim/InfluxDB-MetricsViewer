// See https://aka.ms/new-console-template for more information

using MetaMetrics.Api;
using MetaMetricsViewer.Console.Lib;
using MetaMetricsViewer.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddEnvironmentVariables()
    .Build();

var serviceProvider = new ServiceCollection()
    .Configure<InfluxDBOptions>(configuration.GetSection(InfluxDBOptions.Section))
    .AddSingleton<MetaMetricsClientService>()
    .AddSingleton<InfluxDBService>()
    .AddTransient<IMetaMetricsService, MetaMetricsService>()
    .AddOptions()
    .BuildServiceProvider();



var lib = new InfluxDBTest(serviceProvider);
await lib.TestWithOutValues();

//await lib.TestFilterFromQueryAsync();
//Or
//await lib.TestFilterFromQueryMeasurement();

Console.ReadKey();