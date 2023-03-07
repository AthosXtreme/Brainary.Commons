using Brainary.Commons.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brainary.Commons.Web
{
    public class BackgroundQueueService : BackgroundService
    {
        private readonly BackgroundQueueWorker queue;
        private readonly ILogger<BackgroundQueueService> logger;

        public BackgroundQueueService(BackgroundQueueWorker queue, ILogger<BackgroundQueueService> logger)
        {
            this.queue = queue;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await queue.Dequeue(stoppingToken);

                if (workItem != null)
                {
                    try
                    {
                        await workItem(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An exception occurred in a background queue task.");
                    }
                }
            }
        }
    }
}
