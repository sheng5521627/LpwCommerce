using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Stores
{
    /// <summary>
    /// 实体和商店的映射关系
    /// </summary>
    public partial interface IStoreMappingSupported
    {
        /// <summary>
        /// 实体是否支持商店
        /// </summary>
        bool LimitedToStores { get; set; }
    }
}
