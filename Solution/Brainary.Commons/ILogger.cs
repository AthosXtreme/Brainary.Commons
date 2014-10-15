namespace Brainary.Commons
{
    /// <summary>
    /// Logging interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Information log
        /// </summary>
        /// <param name="message">Message string</param>
        void Info(string message);

        /// <summary>
        /// Error log
        /// </summary>
        /// <param name="message">Message string</param>
        void Error(string message);

        /// <summary>
        /// Fatal log
        /// </summary>
        /// <param name="message">Message string</param>
        void Fatal(string message);

        /// <summary>
        /// Waring log
        /// </summary>
        /// <param name="message">Message string</param>
        void Warn(string message);

        /// <summary>
        /// Debug log
        /// </summary>
        /// <param name="message">Message string</param>
        void Debug(string message);

        /// <summary>
        /// Information log with exception detail
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="exception">Exception</param>
        void Info(string message, System.Exception exception);

        /// <summary>
        /// Error log with exception detail
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="exception">Exception</param>
        void Error(string message, System.Exception exception);

        /// <summary>
        /// Fatal log with exception detail
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="exception">Exception</param>
        void Fatal(string message, System.Exception exception);

        /// <summary>
        /// Warinig log with exception detail
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="exception">Exception</param>
        void Warn(string message, System.Exception exception);

        /// <summary>
        /// Debug log with exception detail
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="exception">Exception</param>
        void Debug(string message, System.Exception exception);
    }
}