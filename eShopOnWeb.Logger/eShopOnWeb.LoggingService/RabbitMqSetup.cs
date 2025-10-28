using RabbitMQ.Client;
using UCL.RabbitMQ.Core;
using UCL.RabbitMQ.Core.Records;
namespace eShopOnWeb.LoggingService
{
  public class RabbitMqSetup : RabbitServiceBase, IHostedService
  {
    //Exchanges
    const string InvalidMessagesX = "invalid_messages.exchange";
    const string DLX = "dead_letter.exchange";
    const string ManualInspectionX = "manual_inspections.exchange";

    //Queues
    const string InvalidMessageQ = "logging.invalid_messages.queue";
    const string DLQ = "logging.dead_letters.queue";
    const string ManualInspectionQ = "logging.manual_inspections.queue";


    private readonly ILogger<RabbitMqSetup> _logger;

    public RabbitMqSetup(ILogger<RabbitMqSetup> logger, string? hostName = null, string? userName = null, string? password = null) : base(hostName, userName, password)
    {
      _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      try
      {
        await InitializeAsync();

        await EstablishExchangesAsync(
        new ExchangeConfig(Name: InvalidMessagesX, Type: "topic", Durable: true, AutoDelete: false),
        new ExchangeConfig(Name: DLX, Type: "topic", Durable: true, AutoDelete: false),
        new ExchangeConfig(Name: ManualInspectionX, Type: "topic", Durable: true, AutoDelete: false));


        await EstablishQueuesAsync(
        new QueueConfig(InvalidMessageQ, Durable: true, Exclusive: false, AutoDelete: false),
        new QueueConfig(DLQ, Durable: true, Exclusive: false, AutoDelete: false),
        new QueueConfig(ManualInspectionQ, Durable: true, Exclusive: false, AutoDelete: false)
        );

        await BindQueuesAsync(
        new BindConfig(Exchange: InvalidMessagesX, RoutingKey: "invalid.#", Queue: InvalidMessageQ, Durable: true, Exclusive: false, AutoDelete: false),
        new BindConfig(Exchange: DLX, RoutingKey: "#", Queue: DLQ, Durable: true, Exclusive: false, AutoDelete: false),
        new BindConfig(Exchange: ManualInspectionX, RoutingKey: "manual.#", Queue: ManualInspectionQ, Durable: true, Exclusive: false, AutoDelete: false));


      }
      catch (Exception e)
      {
        _logger.LogError("Something went wrong setting up RabbitMQ: {Error}", e.Message);
        throw;
      }




    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }




  }
}
