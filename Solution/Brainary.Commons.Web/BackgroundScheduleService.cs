using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brainary.Commons.Web
{
    /// <summary>
    /// Base implementation for scheduling a task at intervals as Hosted Service.
    /// </summary>
    public abstract class BackgroundScheduleService : BackgroundService
    {
        private readonly ILogger<BackgroundScheduleService> logger;

        public BackgroundScheduleService(ILogger<BackgroundScheduleService> logger)
        {
            this.logger = logger;
        }

        protected abstract TimeSpan Interval { get; init; }

        protected abstract Task Action(CancellationToken stoppingToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await Action(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An exception occurred in a background scheduled task.");
                }
            }
        }
    }
}
