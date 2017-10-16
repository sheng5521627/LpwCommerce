using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Catalog
{
    /// <summary>
    /// 代表一个产品评论
    /// </summary>
    public partial class ProductReviewHelpfulness : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product review identifier
        /// </summary>
        public int ProductReviewId { get; set; }

        /// <summary>
        /// A value indicating whether a review a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual ProductReview ProductReview { get; set; }
    }
}
