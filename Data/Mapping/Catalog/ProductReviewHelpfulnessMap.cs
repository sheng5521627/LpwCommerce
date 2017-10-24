using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductReviewHelpfulnessMap : NopEntityTypeConfiguration<ProductReviewHelpfulness>
    {
        public ProductReviewHelpfulnessMap()
        {
            this.ToTable("ProductReviewHelpfulness");
            this.HasKey(m => m.Id);
            this.HasRequired(m => m.ProductReview)
                .WithMany(m => m.ProductReviewHelpfulnessEntries)
                .HasForeignKey(m => m.ProductReviewId);
        }
    }
}
