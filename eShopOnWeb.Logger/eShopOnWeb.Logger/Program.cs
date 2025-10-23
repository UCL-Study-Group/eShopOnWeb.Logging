

using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Values from configuration (appsettings.json)
var elasticUri = builder.Configuration["ElasticsearchConfiguration:Uri"];
var serviceName = builder.Configuration["ElasticsearchConfiguration:ServiceName"];


//Serilog-configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new[] { new Uri("http://localhost:9200") }, opts =>
    {
      // logs i ElasticSearch
      opts.DataStream = new DataStreamName("logs", serviceName.ToLower());
    })
    .MinimumLevel.Information()  // Don't log Debug messages (reduce noise)
    .CreateLogger();

    //brug serilog
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


