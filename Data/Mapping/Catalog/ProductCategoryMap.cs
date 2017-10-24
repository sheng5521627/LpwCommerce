using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductCategoryMap : NopEntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            this.ToTable("Product_Category_Mapping");
            this.HasKey(pc => pc.Id);

            this.HasRequired(m => m.Category)
                .WithMany()
                .HasForeignKey(m => m.CategoryId);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductCategories)
                .HasForeignKey(m => m.ProductId);
        }
    }
}
