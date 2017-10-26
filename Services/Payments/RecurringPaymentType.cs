using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Payments
{
    /// <summary>
    /// Represents a recurring payment type
    /// </summary>
    public enum RecurringPaymentType
    {
        /// <summary>
        /// Not supported
        /// </summary>
        NotSupported = 0,
        /// <summary>
        /// Manual 手工的
        /// </summary>
        Manual = 10,
        /// <summary>
        /// Automatic (payment is processed on payment gateway site)付款是在支付网关站点上处理的。
        /// </summary>
        Automatic = 20
    }
}
