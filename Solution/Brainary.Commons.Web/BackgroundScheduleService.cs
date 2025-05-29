using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brainary.Commons.Web
{
    /// <summary>
    /// Base implementation for scheduling a task at cron intervals as Hosted Service.
    /// Using cron expressions from Cronos library: https://github.com/HangfireIO/Cronos
    /// </summary>
    public abstract class BackgroundScheduleService(ILogger<BackgroundScheduleService> logger) : BackgroundService
    {
        private const string DefaultSchedule = "* */10 * * * *";

        private bool executeImmediate;
        private readonly ILogger logger = logger;

        protected bool ExecuteImmediate { get => executeImmediate; init => executeImmediate = value; }

        protected string CronExpression { get; init; } = DefaultSchedule;

        protected abstract Task Action(CancellationToken stoppingToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CronExpression schedule;
            try
            {
                schedule = Cronos.CronExpression.Parse(CronExpression, CronFormat.IncludeSeconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred parsing schedule in a background scheduled task.");
                return;
            }

            if (executeImmediate)
            {
                logger.LogDebug("Executing scheduled action immediately on start.");
                executeImmediate = false;
                await Execute();
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var current = DateTimeOffset.Now;
                var dtoffset = schedule.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
                if (!dtoffset.HasValue) continue;
                var delay = dtoffset.Value.DateTime - current.DateTime;
                await Task.Delay(delay, stoppingToken);
                await Execute();
            }

            return;

            async Task Execute()
            {
                if (!stoppingToken.IsCancellationRequested)
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
}
