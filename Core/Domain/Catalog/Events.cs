using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Catalog
{
    /// <summary>
    /// 产品评审批准的事件
    /// </summary>
    public class ProductReviewApprovedEvent
    {
        public ProductReviewApprovedEvent(ProductReview productReview)
        {
            this.ProductReview = productReview;
        }

        /// <summary>
        /// Product review
        /// </summary>
        public ProductReview ProductReview { get; private set; }
    }
}
