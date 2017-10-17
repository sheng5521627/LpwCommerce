using Core;
using Core.Data;
using Core.Domain.Common;
using Core.Domain.Customers;
using Core.Domain.Logging;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Logging
{
    public partial class DefaultLogger : ILogger
    {
        #region Fields

        private readonly IRepository<Log> _logRepostiroy;
        private readonly IWebHelper _webHelper;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;

        #endregion

        public DefaultLogger(
            IRepository<Log> logRepository,
            IWebHelper webHelper,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            _logRepostiroy = logRepository;
            _webHelper = webHelper;
            _dbContext = dbContext;
            _dataProvider = dataProvider;
            _commonSettings = commonSettings;
        }

        /// <summary>
        /// 是否忽视记录日志
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool IgnoreLog(string message)
        {
            if (_commonSettings.IgnoreLogWordlist.Count == 0)
                return false;
            if (string.IsNullOrEmpty(message))
                return false;
            return _commonSettings.IgnoreLogWordlist.Any(x => message.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        /// <summary>
        /// 日志级别是否可用
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public virtual bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        public virtual void DeleteLog(Log log)
        {
            if (log == null)
                throw new ArgumentException("log");

            _logRepostiroy.Delete(log);
        }

        public virtual void ClearLog()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                string logTableName = _dbContext.GetTableName<Log>();
                _dbContext.ExecuteSqlCommand(string.Format("TRUNCATE TABLE [{0}]", logTableName));
            }
            else
            {
                var logs = _logRepostiroy.Table.ToList();
                foreach (var log in logs)
                    _logRepostiroy.Delete(log);
            }
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="fromUtc"></param>
        /// <param name="toUts"></param>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUts = null, string message = "", LogLevel? logLevel = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _logRepostiroy.Table;
            if (fromUtc.HasValue)
                query = query.Where(l => l.CreatedOnUtc >= fromUtc.Value);
            if (toUts.HasValue)
                query = query.Where(l => l.CreatedOnUtc <= toUts.Value);
            if (logLevel.HasValue)
            {
                var logLevelId = (int)logLevel.Value;
                query = query.Where(l => l.LogLevelId == logLevelId);
            }
            if (string.IsNullOrEmpty(message))
            {
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            }
            query = query.OrderByDescending(l => l.CreatedOnUtc);
            var logs = new PagedList<Log>(query, pageIndex, pageSize);
            return logs;
        }

        public virtual Log GetLogById(int logId)
        {
            if (logId == 0)
                return null;
            return _logRepostiroy.GetById(logId);
        }

        public virtual IList<Log> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new Log[0];
            var query = from l in _logRepostiroy.Table
                        where logIds.Contains(l.Id)
                        select l;
            var logs = query.ToList();
            var sortedLogItems = new List<Log>();
            foreach (int id in logIds)
            {
                var log = logs.Find(x => x.Id == id);
                if (log != null)
                {
                    sortedLogItems.Add(log);
                }
            }
            return sortedLogItems;
        }

        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            {
                return null;
            }

            var log = new Log()
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                Customer = customer,
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.Now
            };
            _logRepostiroy.Insert(log);
            return log;
        }
    }
}
