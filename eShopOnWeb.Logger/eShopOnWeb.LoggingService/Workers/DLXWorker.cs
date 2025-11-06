using eShopOnWeb.LoggingService.Setup;
using UCL.RabbitMQ.Core.Records;

namespace eShopOnWeb.LoggingService.Workers
{
  public class DLXWorker : BackgroundService
  {
    private readonly ILogger<DLXWorker> _logger;
    private readonly RabbitMqSetup _setup;

    const string DLQ = "logging.dead_letters.queue";

    public DLXWorker(ILogger<DLXWorker> logger, RabbitMqSetup setup)
    {
      _logger = logger;
      _setup = setup;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {

      await base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      try
      {
        _logger.LogInformation("Starting RabbitMQ setup...");
        await _setup.StartAsync(stoppingToken);
        _logger.LogInformation("RabbitMQ setup is ready!");
      }
      catch (Exception ex)
      {
        _logger.LogCritical(ex, "Something went wrong when setting up RabbitMQ. Worker is not able to start");
      }

      var subscription = await _setup.SubscribeAsync(new SubscribeConfig
     (QueueName: DLQ,
     AutoAck: false,
     PrefetchCount: 1));

    }
  }
}
