using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;
using Core.Domain.Common;
using Data;

namespace Services.Common
{
    /// <summary>
    /// Full-Text service
    /// </summary>
    public partial class FulltextService : IFulltextService
    {
        #region Fields

        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        public FulltextService(IDataProvider dataProvider, IDbContext dbContext,
            CommonSettings commonSettings)
        {
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
        }

        public bool IsFullTextSupported()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                var result = _dbContext.SqlQuery<int>("EXEC [FullText_IsSupported]");
                return result.FirstOrDefault() > 0;
            }

            //stored procedures aren't supported
            return false;
        }

        public void EnableFullText()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database.
                _dbContext.ExecuteSqlCommand("EXEC [FullText_Enable]", true);
            }
            else
            {
                throw new Exception("Stored procedures are not supported by your database");
            }
        }

        public void DisableFullText()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database.
                _dbContext.ExecuteSqlCommand("EXEC [FullText_Disable]", true);
            }
            else
            {
                throw new Exception("Stored procedures are not supported by your database");
            }
        }

        #endregion
    }
}
