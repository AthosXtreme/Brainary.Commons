namespace Brainary.Commons
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Windows event log implementation
    /// </summary>
    public class EventLogger : ILogger
    {
        private readonly string source;

        public EventLogger(string source)
        {
            this.source = source;

            if (!EventLog.SourceExists(this.source))
                EventLog.CreateEventSource(this.source, "Application");
        }

        public void Info(string message)
        {
            Log(message, EventLogEntryType.Information, null);
        }

        public void Error(string message)
        {
            Log(message, EventLogEntryType.Error, null);
        }

        public void Fatal(string message)
        {
            Log(message, EventLogEntryType.Error, null);
        }

        public void Warn(string message)
        {
            Log(message, EventLogEntryType.Warning, null);
        }

        public void Debug(string message)
        {
            Log(message, EventLogEntryType.Information, null);
        }

        public void Info(string message, Exception exception)
        {
            Log(message, EventLogEntryType.Information, exception);
        }

        public void Error(string message, Exception exception)
        {
            Log(message, EventLogEntryType.Error, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(message, EventLogEntryType.Error, exception);
        }

        public void Warn(string message, Exception exception)
        {
            Log(message, EventLogEntryType.Warning, exception);
        }

        public void Debug(string message, Exception exception)
        {
            Log(message, EventLogEntryType.Information, exception);
        }

        private void Log(string message, EventLogEntryType severity, Exception exception)
        {
            EventLog.WriteEntry(source, string.Format("{0}\r\n{1}", message, exception != null ? exception.StackTrace : string.Empty), severity);
        }
    }
}
