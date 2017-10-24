using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductReviewMap : NopEntityTypeConfiguration<ProductReview>
    {
        public ProductReviewMap()
        {
            this.ToTable("ProductReview");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductReviews)
                .HasForeignKey(m => m.ProductId);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId);
        }
    }
}
