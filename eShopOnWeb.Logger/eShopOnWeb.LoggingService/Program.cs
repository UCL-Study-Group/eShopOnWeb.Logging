using Serilog;
using Elastic.Serilog.Sinks;
using Elastic.Ingest.Elasticsearch.DataStreams;
using eShopOnWeb.LoggingService.Setup;
using eShopOnWeb.LoggingService.Workers;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSingleton<RabbitMqSetup>();

builder.Services.AddHostedService<InvalidMessageWorker>();

builder.Services.AddHostedService<DeadLetterWorker>();

//brug serilog
builder.Services.AddSerilog();

var host = builder.Build();

//Values from configuration (appsettings.json)
var elasticUri = builder.Configuration["ElasticsearchConfiguration:Uri"];
var serviceName = builder.Configuration["ElasticsearchConfiguration:ServiceName"] ?? "LoggingService";


//Serilog-configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new[] { new Uri(elasticUri ?? "http://localhost:9200") }, opts =>
    {
      // logs i ElasticSearch
      opts.DataStream = new DataStreamName("logs", serviceName.ToLower());
    })
    .MinimumLevel.Information()  //så logger vi ikke debugs
    .CreateLogger();


await host.RunAsync();
