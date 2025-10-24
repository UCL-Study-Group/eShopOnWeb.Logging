using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopOnWeb.LoggingService
{
  public class RabbitMqSetup : IHostedService
  {
    private readonly ILogger<RabbitMqSetup> _logger;
    private readonly IConnection _connection;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
