using eShopOnWeb.LoggingService.Setup;
using System.Text;
using UCL.RabbitMQ.Core.Records;

namespace eShopOnWeb.LoggingService.Workers
{
  public class DeadLetterWorker : BackgroundService
  {
    private readonly ILogger<DeadLetterWorker> _logger;
    private readonly RabbitMqSetup _setup;

    const string DLQ = "logging.dead_letters.queue";

    public DeadLetterWorker(ILogger<DeadLetterWorker> logger, RabbitMqSetup setup)
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
     PrefetchCount: 1
     ));

      subscription.MessageReceived += (async (sender, ea) =>
      {
        try
        {
          string body = Encoding.UTF8.GetString(ea.Body.ToArray());

          var headers = ea.BasicProperties.Headers;

          _logger.LogError("Dead Letter Received: Message: {Body} with Headers: {@Headers}", body, headers);

          await subscription.Channel.BasicAckAsync(ea.DeliveryTag, false);

          await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception e)
        {
          _logger.LogError(e, "Something went wrong when logging a dead letter");

          await subscription.Channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }

      });
    }
  }
}
