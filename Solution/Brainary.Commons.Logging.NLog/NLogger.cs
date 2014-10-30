namespace Brainary.Commons.Logging.NLog
{
    using System;

    using global::NLog;

    /// <summary>
    /// NLog implementation of <see cref="ILogger"/>
    /// </summary>
    public class NLogger : ILogger
    {
        private readonly Logger instance;

        public NLogger()
        {
            instance = LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            instance.Info(message);
        }

        public void Error(string message)
        {
            instance.Error(message);
        }

        public void Fatal(string message)
        {
            instance.Fatal(message);
        }

        public void Warn(string message)
        {
            instance.Warn(message);
        }

        public void Debug(string message)
        {
            instance.Debug(message);
        }

        public void Info(string message, Exception exception)
        {
            instance.Info(message, exception);
        }

        public void Error(string message, Exception exception)
        {
            instance.Error(message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            instance.Fatal(message, exception);
        }

        public void Warn(string message, Exception exception)
        {
            instance.Warn(message, exception);
        }

        public void Debug(string message, Exception exception)
        {
            instance.Debug(message, exception);
        }
    }
}
