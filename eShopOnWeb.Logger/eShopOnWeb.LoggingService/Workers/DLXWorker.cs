using eShopOnWeb.LoggingService.Setup;

namespace eShopOnWeb.LoggingService.Workers
{
  public class DLXWorker : BackgroundService
  {
    private readonly ILogger<DLXWorker> _logger;
    private readonly RabbitMqSetup _setup;

    public DLXWorker(ILogger<DLXWorker> logger, RabbitMqSetup setup)
    {
      _logger = logger;
      _setup = setup;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("RabbitMQ worker starting");

      await base.StartAsync(cancellationToken);
    }

                  
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      throw new NotImplementedException();
    }
  }
}
