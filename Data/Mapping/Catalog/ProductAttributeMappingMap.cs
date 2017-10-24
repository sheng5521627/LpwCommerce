using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductAttributeMappingMap:NopEntityTypeConfiguration<ProductAttributeMapping>
    {
        public ProductAttributeMappingMap()
        {
            this.ToTable("Product_ProductAttribute_Mapping");
            this.HasKey(m => m.Id);
            this.Ignore(m => m.AttributeControlType);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductAttributeMappings)
                .HasForeignKey(m => m.ProductId);

            this.HasRequired(m => m.ProductAttribute)
                .WithMany()
                .HasForeignKey(m => m.ProductAttributeId);
        }
    }
}
