namespace Brainary.Commons.Logging.Log4Net
{
    using System;
    using System.Reflection;
    using log4net;
    using log4net.Config;

    /// <summary>
    /// Log4Net implementation of <see cref="ILogger"/>
    /// </summary>
    public class L4NLogger : ILogger
    {
        private readonly ILog instance;

        public L4NLogger()
        {
            XmlConfigurator.Configure();
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            instance = LogManager.GetLogger(assembly.GetType());
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
