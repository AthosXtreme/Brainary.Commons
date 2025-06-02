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
        private const string MinuteControlFormat = "yyyyMMddHHmm";

        private bool minuteControlEnabled;
        private string minuteControlStamp = string.Empty;
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
                var secondExpression = CronExpression.Split(' ')[0];
                minuteControlEnabled = secondExpression == "*" || secondExpression.Contains("second");

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
                await RunAction(stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var current = DateTime.UtcNow;
                var next = schedule.GetNextOccurrence(current);
                if (!next.HasValue || (minuteControlEnabled && next.Value.ToString(MinuteControlFormat) == minuteControlStamp)) continue;
                var delay = next - current;
                await Task.Delay(delay.Value, stoppingToken);
                await RunAction(stoppingToken);
            }
        }

        private async Task RunAction(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    minuteControlStamp = DateTime.UtcNow.ToString(MinuteControlFormat);
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
