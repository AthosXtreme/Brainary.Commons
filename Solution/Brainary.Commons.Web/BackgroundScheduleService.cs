using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brainary.Commons.Web
{
    /// <summary>
    /// Base implementation for scheduling a task at intervals as Hosted Service.
    /// </summary>
    public abstract class BackgroundScheduleService : BackgroundService
    {
        private bool executeImmediate;
        private readonly ILogger logger;

        public BackgroundScheduleService(ILogger<BackgroundScheduleService> logger)
        {
            this.logger = logger;
        }

        protected bool ExecuteImmediate { get => executeImmediate; init => executeImmediate = value; }

        protected TimeSpan Interval { get; init; } = TimeSpan.FromMinutes(1);

        protected abstract Task Action(CancellationToken stoppingToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            async Task execute()
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

            if (executeImmediate)
            {
                logger.LogDebug("Executing scheduled action immediately.");
                await execute();
                executeImmediate = false;
            }

            using var timer = new PeriodicTimer(Interval);
            logger.LogDebug($"Periodic timer set for {Interval}");
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                logger.LogDebug("Executing scheduled action.");
                await execute();
            }
        }
    }
}
