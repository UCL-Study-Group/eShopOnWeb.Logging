using eShopOnWeb.LoggingService.Helpers;
using eShopOnWeb.LoggingService.Models;
using eShopOnWeb.LoggingService.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCL.RabbitMQ.Core;
using UCL.RabbitMQ.Core.Records;


namespace eShopOnWeb.LoggingService.Workers
{
  public class InvalidMessageWorker : BackgroundService
  {
    private readonly ILogger<InvalidMessageWorker> _logger;
    private readonly RabbitMqSetup _setup;


    private const string InvalidMessageQ = "logging.invalid_messages.queue";

    public InvalidMessageWorker(ILogger<InvalidMessageWorker> logger, RabbitMqSetup setup)
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
      (QueueName: InvalidMessageQ,
      AutoAck: false,
      PrefetchCount: 1));

      subscription.MessageReceived += (async (sender, ea) =>
      {
        try
        {
          //Try to fetch correlationId, but should always be set 
          string? correlationId = null;
          if (ea.BasicProperties.Headers is not null &&
              ea.BasicProperties.Headers.TryGetValue("CorrelationId", out var correlationIdObj))
          {
            correlationId = Encoding.UTF8.GetString((byte[])correlationIdObj) ?? "N/A";
          }

          InvalidMessageReport? report = MessageHandler.TryUnpack<InvalidMessageReport>(ea.Body, _logger);

          if (report == null)
          {
            _logger.LogWarning("CorrelationID ({CorrelationId}): Corrupt report received on {Queue}. Message will be Nack'ed.",
            correlationId, InvalidMessageQ);
            await subscription.Channel.BasicNackAsync(ea.DeliveryTag, false, false);
            return;
          }

          var logEntry = new InvalidMessageLogEntry(
            Timestamp: DateTime.UtcNow,
            LogLevel: "Warning",
            CorrelationId: correlationId,
            ReportingService: report.ReportingService ?? "Uknown",
            ErrorMessage: report.ErrorMessage ?? "Uknown",
            OriginalMessageBody: report.OriginalMessageBody ?? "Uknown");

          _logger.LogWarning("Invalid message reported: {JsonLog}", logEntry.ToJson());

          await subscription.Channel.BasicAckAsync(ea.DeliveryTag, false);

        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Critical failure in Invalid Message Worker. Message will be Nack'ed.");

          await subscription.Channel.BasicNackAsync(ea.DeliveryTag, false, false);
        }
      });
    }
  }
}
