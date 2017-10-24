using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductSpecificationAttributeMap : NopEntityTypeConfiguration<ProductSpecificationAttribute>
    {
        public ProductSpecificationAttributeMap()
        {
            this.ToTable("Product_SpecificationAttribute_Mapping");
            this.HasKey(m => m.Id);
            this.Property(m => m.CustomValue).HasMaxLength(4000);
            this.Ignore(m => m.AttributeType);

            this.HasRequired(m => m.SpecificationAttributeOption)
                .WithMany()
                .HasForeignKey(m => m.SpecificationAttributeOptionId);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductSpecificationAttributes)
                .HasForeignKey(m => m.ProductId);
        }
    }
}
