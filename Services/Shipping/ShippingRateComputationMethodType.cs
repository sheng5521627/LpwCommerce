using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Shipping
{
    /// <summary>
    /// Represents a shipping rate computation method type
    /// 表示运费率计算方法类型。
    /// </summary>
    public enum ShippingRateComputationMethodType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Offline
        /// </summary>
        Offline = 10,
        /// <summary>
        /// Realtime
        /// </summary>
        Realtime = 20
    }
}
