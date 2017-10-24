using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class GiftCardMap : NopEntityTypeConfiguration<GiftCard>
    {
        public GiftCardMap()
        {
            this.ToTable("GiftCard");
            this.HasKey(gc => gc.Id);

            this.Property(gc => gc.Amount).HasPrecision(18, 4);

            this.Ignore(gc => gc.GiftCardType);

            this.HasOptional(m => m.PurchasedWithOrderItem)
                .WithMany(m => m.AssociatedGiftCards)
                .HasForeignKey(m => m.PurchasedWithOrderItemId);
        }
    }
}
