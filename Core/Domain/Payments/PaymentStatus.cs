using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Payments
{
    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Pending 待定的
        /// </summary>
        Pending = 10,
        /// <summary>
        /// Authorized 审批
        /// </summary>
        Authorized = 20,
        /// <summary>
        /// Paid 支付
        /// </summary>
        Paid = 30,
        /// <summary>
        /// Partially Refunded 部分退款
        /// </summary>
        PartiallyRefunded = 35,
        /// <summary>
        /// Refunded 退款
        /// </summary>
        Refunded = 40,
        /// <summary>
        /// Voided 作废
        /// </summary>
        Voided = 50,
    }
}
