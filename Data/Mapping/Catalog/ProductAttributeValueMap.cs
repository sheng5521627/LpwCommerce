using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductAttributeValueMap : NopEntityTypeConfiguration<ProductAttributeValue>
    {
        public ProductAttributeValueMap()
        {
            this.ToTable("ProductAttributeValue");
            this.HasKey(pav => pav.Id);
            this.Property(pav => pav.Name).IsRequired().HasMaxLength(400);
            this.Property(pav => pav.ColorSquaresRgb).HasMaxLength(100);

            this.Property(pav => pav.PriceAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.WeightAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.Cost).HasPrecision(18, 4);

            this.Ignore(pav => pav.AttributeValueType);

            this.HasRequired(m => m.ProductAttributeMapping)
                .WithMany(m => m.ProductAttributeValues)
                .HasForeignKey(m => m.ProductAttributeMappingId);
        }
    }
}
