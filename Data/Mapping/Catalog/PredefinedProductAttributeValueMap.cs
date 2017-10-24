using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class PredefinedProductAttributeValueMap:NopEntityTypeConfiguration<PredefinedProductAttributeValue>
    {
        public PredefinedProductAttributeValueMap()
        {
            this.ToTable("PredefinedProductAttributeValue");
            this.HasKey(pav => pav.Id);
            this.Property(pav => pav.Name).IsRequired().HasMaxLength(400);

            this.Property(pav => pav.PriceAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.WeightAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.Cost).HasPrecision(18, 4);

            this.HasRequired(pav => pav.ProductAttribute)
                .WithMany()
                .HasForeignKey(m => m.ProductAttributeId);
        }
    }
}
