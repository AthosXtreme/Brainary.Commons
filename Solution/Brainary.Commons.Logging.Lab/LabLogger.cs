namespace Brainary.Commons.Logging.Lab
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Microsoft Logging Application Block implementation of <see cref="ILogger"/>
    /// </summary>
    public class LabLogger : ILogger
    {
        #region ILogger implementation

        public void Info(string message)
        {
            Log(message, TraceEventType.Information, null);
        }

        public void Error(string message)
        {
            Log(message, TraceEventType.Error, null);
        }

        public void Fatal(string message)
        {
            Log(message, TraceEventType.Critical, null);
        }

        public void Warn(string message)
        {
            Log(message, TraceEventType.Warning, null);
        }

        public void Debug(string message)
        {
            Log(message, TraceEventType.Verbose, null);
        }

        public void Info(string message, Exception exception)
        {
            Log(message, TraceEventType.Information, exception);
        }

        public void Error(string message, Exception exception)
        {
            Log(message, TraceEventType.Error, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(message, TraceEventType.Critical, exception);
        }

        public void Warn(string message, Exception exception)
        {
            Log(message, TraceEventType.Warning, exception);
        }

        public void Debug(string message, Exception exception)
        {
            Log(message, TraceEventType.Verbose, exception);
        }

        #endregion

        private static void Log(string message, TraceEventType severity, Exception exception)
        {
            var logEntry = new LogEntry
            {
                Severity = severity,
                Title = string.Format("{0} log entry", GetAppName()),
                Priority = GetPriority(severity),
                Message = message,
                Categories = { GetCategory(severity) }
            };

            if (exception != null)
            {
                logEntry.ExtendedProperties.Add("Source", exception.Source);
                logEntry.ExtendedProperties.Add("StackTrace", exception);

                if (exception.InnerException != null)
                    logEntry.ExtendedProperties.Add("InnerExceptionMessage", exception.InnerException.Message);
            }

            Logger.Write(logEntry);
            Logger.FlushContextItems();
        }

        private static int GetPriority(TraceEventType severity)
        {
            switch (severity)
            {
                case TraceEventType.Critical:
                    return 5;
                case TraceEventType.Error:
                    return 4;
                case TraceEventType.Warning:
                    return 3;
                case TraceEventType.Information:
                    return 2;
                case TraceEventType.Verbose:
                    return 1;
                default:
                    return 0;
            }
        }

        private static string GetCategory(TraceEventType severity)
        {
            switch (severity)
            {
                case TraceEventType.Critical:
                    return "Fatal";
                case TraceEventType.Error:
                    return "Error";
                case TraceEventType.Warning:
                    return "Warn";
                case TraceEventType.Information:
                    return "Info";
                case TraceEventType.Verbose:
                    return "Debug";
                default:
                    return "Other";
            }
        }

        private static string GetAppName()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetName().Name;
        }
    }
}