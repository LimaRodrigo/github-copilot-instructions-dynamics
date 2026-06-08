using System;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace Account.Extend.Infrastructure
{
    /// <summary>
    /// Adaptador que implementa ILogger usando o serviço de tracing do Dynamics 365.
    /// Permite usar a abstração ILogger do .NET enquanto integra com o tracing nativo do plugin.
    /// </summary>
    public class LoggerAdapter : ILogger
    {
        private readonly ITracingService _tracingService;
        private readonly string _categoryName;

        public LoggerAdapter(ITracingService tracingService, string categoryName)
        {
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) => NoOpDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            string message = formatter(state, exception);
            
            if (string.IsNullOrEmpty(message) && exception == null)
                return;

            string logMessage = FormatLogMessage(logLevel, message, exception);
            _tracingService.Trace(logMessage);
        }

        private string FormatLogMessage(LogLevel logLevel, string message, Exception exception)
        {
            var logMessageBuilder = new System.Text.StringBuilder();
            logMessageBuilder.AppendFormat("[{0}] [{1}] {2}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), logLevel, message);

            if (exception != null)
            {
                logMessageBuilder.AppendLine();
                logMessageBuilder.AppendFormat("Exception: {0}", exception);
            }

            return logMessageBuilder.ToString();
        }

        private class NoOpDisposable : IDisposable
        {
            public static readonly NoOpDisposable Instance = new NoOpDisposable();

            public void Dispose()
            {
            }
        }
    }
}
