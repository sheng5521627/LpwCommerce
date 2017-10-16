using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum search type
    /// </summary>
    public enum ForumSearchType
    {
        /// <summary>
        /// Topic titles and post text
        /// </summary>
        All = 0,
        /// <summary>
        /// Topic titles only
        /// </summary>
        TopicTitlesOnly = 10,
        /// <summary>
        /// Post text only
        /// </summary>
        PostTextOnly = 20,
    }
}
