using Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Web.Hosting;
using Data.Initializers;
using System.Data.SqlClient;

namespace Data
{
    public class SqlServerDataProvider : IDataProvider
    {
        #region Utilities

        protected virtual string[] ParseComands(string filePath, bool throwExceptionIfNotExsits)
        {
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNotExsits)
                    throw new ArgumentException("文件不存在");
                return new string[0];
            }

            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    string statement;
                    while ((statement = ReadNextStatementFromStream(reader)) != null)
                    {
                        statements.Add(statement);
                    }
                }
            }
            return statements.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    return null;
                }

                if (lineOfText.Trim().ToUpper() == "GO")
                    break;
                sb.Append(lineOfText + Environment.NewLine);
            }
            return sb.ToString();
        }

        #endregion

        public bool StoredProceduredSupported
        {
            get
            {
                return true;
            }
        }

        public DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        public void InitConnectionFactory()
        {
            var connectionFactory = new SqlConnectionFactory();
            Database.DefaultConnectionFactory = connectionFactory;
        }

        public void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetDatabaseInitializer()
        {
            var tableNamesValidate = new[] { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" };
            var customCommands = new List<string>();
            customCommands.AddRange(ParseComands(HostingEnvironment.MapPath("~/App_Data/Install/SqlServer.Indexes.sql"), false));
            customCommands.AddRange(ParseComands(HostingEnvironment.MapPath("~/App_Data/Install/SqlServer.StoredProcedures.sql"), false));

            var initializer = new CreateTablesIfNotExist<NopObjectContext>(tableNamesValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }
    }
}
