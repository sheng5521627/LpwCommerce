using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum topic type
    /// </summary>
    public enum ForumTopicType
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal = 10,
        /// <summary>
        /// Sticky
        /// </summary>
        Sticky = 15,
        /// <summary>
        /// Announcement
        /// </summary>
        Announcement = 20,
    }
}
