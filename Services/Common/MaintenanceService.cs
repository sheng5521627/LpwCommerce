using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Core;
using Core.Data;
using Core.Domain.Common;
using Data;

namespace Services.Common
{
    /// <summary>
    ///  Maintenance service
    /// </summary>
    public partial class MaintenanceService : IMaintenanceService
    {
        #region Fields

        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly HttpContextBase _httpContext;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="httpContext">HTTP context</param>
        public MaintenanceService(IDataProvider dataProvider, IDbContext dbContext,
            CommonSettings commonSettings, HttpContextBase httpContext)
        {
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._httpContext = httpContext;
        }

        #region Utilities

        protected virtual string GetBackupDirectoryPath()
        {
            return string.Format("{0}Administration\\db_backups\\", _httpContext.Request.PhysicalApplicationPath);
        }

        protected virtual void CheckBackupSupported()
        {
            if (_dataProvider.BackupSupported) return;

            throw new DataException("This database does not support backup");
        }

        #endregion

        public int? GetTableIdent<T>() where T : BaseEntity
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                var tableName = _dbContext.GetTableName<T>();
                var result = _dbContext.SqlQuery<decimal>(string.Format("SELECT IDENT_CURRENT('[{0}]')", tableName));
                return Convert.ToInt32(result.FirstOrDefault());
            }

            return null;
        }

        public void SetTableIdent<T>(int ident) where T : BaseEntity
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database.

                var currentIdent = GetTableIdent<T>();
                if (currentIdent.HasValue && ident > currentIdent.Value)
                {
                    var tableName = _dbContext.GetTableName<T>();
                    _dbContext.ExecuteSqlCommand(string.Format("DBCC CHECKIDENT([{0}], RESEED, {1})", tableName, ident));
                }
            }
            else
            {
                throw new Exception("Stored procedures are not supported by your database");
            }
        }

        public IList<FileInfo> GetAllBackupFiles()
        {
            var path = GetBackupDirectoryPath();

            if (!System.IO.Directory.Exists(path))
            {
                throw new IOException("Backup directory not exists");
            }

            return System.IO.Directory.GetFiles(path, "*.bak").Select(fullPath => new FileInfo(fullPath)).OrderByDescending(p => p.CreationTime).ToList();
        }

        public void BackupDatabase()
        {
            CheckBackupSupported();
            var fileName = string.Format(
                "{0}database_{1}_{2}.bak",
                GetBackupDirectoryPath(),
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                CommonHelper.GenerateRandomDigitCode(10));

            var commandText = string.Format(
                "BACKUP DATABASE [{0}] TO DISK = '{1}' WITH FORMAT",
                _dbContext.DbName(),
                fileName);


            _dbContext.ExecuteSqlCommand(commandText, true);
        }

        public void RestoreDatabase(string backupFileName)
        {
            CheckBackupSupported();
            var settings = new DataSettingsManager();
            var conn = new SqlConnectionStringBuilder(settings.LoadSettings().DataConnectionString)
            {
                InitialCatalog = "master"
            };

            //this method (backups) works only with SQL Server database
            using (var sqlConnectiononn = new SqlConnection(conn.ToString()))
            {
                var commandText = string.Format(
                    "DECLARE @ErrorMessage NVARCHAR(4000)\n" +
                    "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE\n" +
                    "BEGIN TRY\n" +
                        "RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE\n" +
                    "END TRY\n" +
                    "BEGIN CATCH\n" +
                        "SET @ErrorMessage = ERROR_MESSAGE()\n" +
                    "END CATCH\n" +
                    "ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE\n" +
                    "IF (@ErrorMessage is not NULL)\n" +
                    "BEGIN\n" +
                        "RAISERROR (@ErrorMessage, 16, 1)\n" +
                    "END",
                    _dbContext.DbName(),
                    backupFileName);

                DbCommand dbCommand = new SqlCommand(commandText, sqlConnectiononn);
                if (sqlConnectiononn.State != ConnectionState.Open)
                    sqlConnectiononn.Open();
                dbCommand.ExecuteNonQuery();
            }

            //clear all pools
            SqlConnection.ClearAllPools();
        }

        public string GetBackupPath(string backupFileName)
        {
            return Path.Combine(GetBackupDirectoryPath(), backupFileName);
        }

        #endregion
    }
}
