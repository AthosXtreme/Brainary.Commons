using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brainary.Commons.Web
{
    /// <summary>
    /// Base implementation for scheduling a task at cron intervals as Hosted Service.
    /// Using cron expressions from Cronos library: https://github.com/HangfireIO/Cronos
    /// </summary>
    public abstract class BackgroundScheduleService : BackgroundService
    {
        private const string defaultSchedule = "* */10 * * * *";

        private bool executeImmediate;
        private readonly ILogger logger;

        public BackgroundScheduleService(ILogger<BackgroundScheduleService> logger)
        {
            this.logger = logger;
        }

        protected bool ExecuteImmediate { get => executeImmediate; init => executeImmediate = value; }

        protected string CronExpression { get; init; } = defaultSchedule;

        protected abstract Task Action(CancellationToken stoppingToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            async Task execute()
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

            CronExpression? schedule = null;
            try
            {
                schedule = Cronos.CronExpression.Parse(CronExpression, CronFormat.IncludeSeconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred parsing schedule in a background scheduled task.");
            }

            if (schedule != null)
            {
                if (executeImmediate)
                {
                    logger.LogDebug("Executing scheduled action immediately on start.");
                    executeImmediate = false;
                    await execute();
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    var current = DateTimeOffset.Now;
                    var dtoffset = schedule.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
                    if (dtoffset.HasValue)
                    {
                        var delay = dtoffset.Value.DateTime - current.DateTime;
                        await Task.Delay(delay, stoppingToken);
                        await execute();
                    }
                } 
            }
        }
    }
}
