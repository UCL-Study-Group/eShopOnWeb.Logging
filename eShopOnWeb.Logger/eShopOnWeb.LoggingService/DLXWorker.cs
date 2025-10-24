
namespace eShopOnWeb.LoggingService
{
    public class DLXWorker : IHostedService
    {
        private readonly ILogger<DLXWorker> _logger;

        public DLXWorker(ILogger<DLXWorker> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
