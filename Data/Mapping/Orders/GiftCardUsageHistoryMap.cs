using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class GiftCardUsageHistoryMap : NopEntityTypeConfiguration<GiftCardUsageHistory>
    {
        public GiftCardUsageHistoryMap()
        {
            this.ToTable("GiftCardUsageHistory");
            this.HasKey(gcuh => gcuh.Id);
            this.Property(gcuh => gcuh.UsedValue).HasPrecision(18, 4);
            //this.Property(gcuh => gcuh.UsedValueInCustomerCurrency).HasPrecision(18, 4);

            this.HasRequired(m => m.GiftCard)
                .WithMany(m => m.GiftCardUsageHistory)
                .HasForeignKey(m => m.GiftCardId);

            this.HasRequired(m => m.UsedWithOrder)
                .WithMany(m => m.GiftCardUsageHistory)
                .HasForeignKey(m => m.UsedWithOrderId);
        }
    }
}
