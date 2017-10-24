using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class CheckoutAttributeValueMap : NopEntityTypeConfiguration<CheckoutAttributeValue>
    {
        public CheckoutAttributeValueMap()
        {
            this.ToTable("CheckoutAttributeValue");
            this.HasKey(cav => cav.Id);
            this.Property(cav => cav.Name).IsRequired().HasMaxLength(400);
            this.Property(cav => cav.ColorSquaresRgb).HasMaxLength(100);
            this.Property(cav => cav.PriceAdjustment).HasPrecision(18, 4);
            this.Property(cav => cav.WeightAdjustment).HasPrecision(18, 4);

            this.HasRequired(m => m.CheckoutAttribute)
                .WithMany(m => m.CheckoutAttributeValues)
                .HasForeignKey(m => m.CheckoutAttributeId);
        }
    }
}
