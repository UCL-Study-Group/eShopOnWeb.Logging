using eShopOnWeb.LoggingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCL.RabbitMQ.Core;
using UCL.RabbitMQ.Core.Records;


namespace eShopOnWeb.LoggingService
{
  public class InvalidMessageWorker : BackgroundService
  {
    private readonly ILogger<InvalidMessageWorker> _logger;
    private readonly RabbitMqSetup _setup;


    private const string InvalidMessageQ = "logging.invalid_messages.queue";
    private const string InvalidMessagesX = "invalid_messages.exchange";

    public InvalidMessageWorker(ILogger<InvalidMessageWorker> logger, RabbitMqSetup setup)
    {
      _logger = logger;
      _setup = setup;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("RabbitMQ worker starting");

      await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var subscription = await _setup.SubscribeAsync(new SubscribeConfig
      (QueueName: InvalidMessageQ,
      AutoAck: false,
      PrefetchCount: 1));

      subscription.MessageReceived += (async (sender, ea) =>
      {
        try
        {
          //Try to fetch correlationId
          string? correlationId = null;
          if (ea.BasicProperties.Headers is not null &&
              ea.BasicProperties.Headers.TryGetValue("CorrelationId", out var correlationIdObj))
          {
            correlationId = Encoding.UTF8.GetString((byte[])correlationIdObj) ?? "N/A";
          }

          InvalidMessageReport? report = MessageHandler.TryUnpack<InvalidMessageReport>(ea.Body, _logger);

          if (report == null)
          {
            _logger.LogWarning("CorrelationID ({CorrelationId}): Corrupt report received on {Queue}. Message will be Nack'ed.", ea.BasicProperties.CorrelationId, InvalidMessageQ);
            await subscription.Channel.BasicNackAsync(ea.DeliveryTag, false, false);
            return;
          }

          var logEntry = new InvalidMessageLogEntry(
            Timestamp: DateTime.UtcNow,
            LogLevel: "Warning",
            CorrelationId: correlationId,
            ReportingService: report.ReportingService ?? "Uknown",
            ErrorMessage: report.ErrorMessage ?? "Uknown",
            OriginalMessageBody: report.OriginalMessageBody);

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
