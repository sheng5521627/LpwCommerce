using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class TierPriceMap : NopEntityTypeConfiguration<TierPrice>
    {
        public TierPriceMap()
        {
            this.ToTable("TierPrice");
            this.HasKey(m => m.Id);
            this.Property(m => m.Price).HasPrecision(18, 4);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.TierPrices)
                .HasForeignKey(m => m.ProductId);

            this.HasRequired(m => m.CustomerRole)
                .WithMany()
                .HasForeignKey(m => m.CustomerRoleId)
                .WillCascadeOnDelete(true);
        }
    }
}
