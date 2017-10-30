using Core;
using Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        public EfDataProviderManager(DataSettings settings)
            : base(settings)
        {

        }

        public override IDataProvider LoadDataProvider()
        {
            var providerName = DataSettings.DataProvider;
            if (string.IsNullOrEmpty(providerName))
                throw new NopException("没有数据提供名称");

            switch (providerName)
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                default:
                    throw new NopException("不支持该名称.");
            }
        }
    }
}
