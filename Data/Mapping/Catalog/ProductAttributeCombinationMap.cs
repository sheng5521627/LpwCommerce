using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductAttributeCombinationMap : NopEntityTypeConfiguration<ProductAttributeCombination>
    {
        public ProductAttributeCombinationMap()
        {
            this.ToTable("ProductAttributeCombination");
            this.HasKey(pac => pac.Id);

            this.Property(pac => pac.Sku).HasMaxLength(400);
            this.Property(pac => pac.ManufacturerPartNumber).HasMaxLength(400);
            this.Property(pac => pac.Gtin).HasMaxLength(400);
            this.Property(pac => pac.OverriddenPrice).HasPrecision(18, 4);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductAttributeCombinations)
                .HasForeignKey(m => m.ProductId);
        }
    }
}
