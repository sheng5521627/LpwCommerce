using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Messages
{
    /// <summary>
    /// Represents priority of queued email
    /// </summary>
    public enum QueuedEmailPriority
    {
        /// <summary>
        /// Low
        /// </summary>
        Low = 0,
        /// <summary>
        /// High
        /// </summary>
        High = 5
    }
}
