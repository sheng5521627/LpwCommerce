using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Customers;
using Core.Domain.Logging;

namespace Services.Logging
{
    public partial class NullLogger : ILogger
    {
        public void ClearLog()
        {
            
        }

        public void DeleteLog(Log log)
        {
            
        }

        public IPagedList<Log> GetAllLogs(DateTime? fromUtc = default(DateTime?), DateTime? toUtc = default(DateTime?), string message = "", LogLevel? logLevel = default(LogLevel?), int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return null;
        }

        public Log GetLogById(int logId)
        {
            return null;
        }

        public IList<Log> GetLogByIds(int[] logIds)
        {
            return null;
        }

        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            return null;
        }

        public bool IsEnabled(LogLevel level)
        {
            return false;
        }
    }
}
