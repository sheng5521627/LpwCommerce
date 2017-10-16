using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Blogs
{
    /// <summary>
    /// Blog post comment approved event
    /// </summary>
    public class BlogCommentApprovedEvent
    {
        public BlogCommentApprovedEvent(BlogComment blogComment)
        {
            this.BlogComment = blogComment;
        }

        /// <summary>
        /// Blog post comment
        /// </summary>
        public BlogComment BlogComment { get; private set; }
    }
}
