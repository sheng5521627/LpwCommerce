using Core.Domain.Customers;
using Core.Domain.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogger logger, string message, Exception exception = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, customer);
        }

        public static void Information(this ILogger logger, string message, Exception excepiton = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Information, message, excepiton, customer);
        }

        public static void Warning(this ILogger logger, string message, Exception excepiton = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, excepiton, customer);
        }

        public static void Error(this ILogger logger, string message, Exception excepiton = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Error, message, excepiton, customer);
        }

        public static void Fatal(this ILogger logger, string message, Exception excepiton = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, excepiton, customer);
        }

        private static void FilteredLog(ILogger logger, LogLevel logLevel, string message, Exception exception = null, Customer customer = null)
        {
            if(exception is System.Threading.ThreadAbortException)
            {
                return;
            }
            if (logger.IsEnabled(logLevel))
            {
                string fullMessage = exception == null ? string.Empty : exception.Message;
                logger.InsertLog(logLevel, message, fullMessage, customer);
            }
        }
    }
}
