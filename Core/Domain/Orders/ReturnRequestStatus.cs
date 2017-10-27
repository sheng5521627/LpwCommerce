using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum ReturnRequestStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 0,
        /// <summary>
        /// Received
        /// </summary>
        Received = 10,
        /// <summary>
        /// Return authorized 经授权的
        /// </summary>
        ReturnAuthorized = 20,
        /// <summary>
        /// Item(s) repaired 修复
        /// </summary>
        ItemsRepaired = 30,
        /// <summary>
        /// Item(s) refunded 退还
        /// </summary>
        ItemsRefunded = 40,
        /// <summary>
        /// Request rejected 拒绝
        /// </summary>
        RequestRejected = 50,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 60,
    }
}
