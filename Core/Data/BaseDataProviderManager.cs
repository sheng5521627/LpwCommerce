using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    /// <summary>
    /// 数据库提供器基类
    /// </summary>
    public abstract class BaseDataProviderManager
    {
        protected BaseDataProviderManager(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentException("settings");
            this.DataSettings = settings;
        }

        protected DataSettings DataSettings { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDataProvider LoadDataProvider();
    }
}
