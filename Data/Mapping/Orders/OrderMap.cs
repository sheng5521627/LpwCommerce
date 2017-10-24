using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class OrderMap : NopEntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            this.ToTable("Order");
            this.HasKey(o => o.Id);
            this.Property(o => o.CurrencyRate).HasPrecision(18, 8);
            this.Property(o => o.OrderSubtotalInclTax).HasPrecision(18, 4);
            this.Property(o => o.OrderSubtotalExclTax).HasPrecision(18, 4);
            this.Property(o => o.OrderSubTotalDiscountInclTax).HasPrecision(18, 4);
            this.Property(o => o.OrderSubTotalDiscountExclTax).HasPrecision(18, 4);
            this.Property(o => o.OrderShippingInclTax).HasPrecision(18, 4);
            this.Property(o => o.OrderShippingExclTax).HasPrecision(18, 4);
            this.Property(o => o.PaymentMethodAdditionalFeeInclTax).HasPrecision(18, 4);
            this.Property(o => o.PaymentMethodAdditionalFeeExclTax).HasPrecision(18, 4);
            this.Property(o => o.OrderTax).HasPrecision(18, 4);
            this.Property(o => o.OrderDiscount).HasPrecision(18, 4);
            this.Property(o => o.OrderTotal).HasPrecision(18, 4);
            this.Property(o => o.RefundedAmount).HasPrecision(18, 4);

            this.Ignore(o => o.OrderStatus);
            this.Ignore(o => o.PaymentStatus);
            this.Ignore(o => o.ShippingStatus);
            this.Ignore(o => o.CustomerTaxDisplayType);
            this.Ignore(o => o.TaxRatesDictionary);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId);

            this.HasRequired(m => m.BillingAddress)
                .WithMany()
                .HasForeignKey(m => m.BillingAddressId)
                .WillCascadeOnDelete(false);

            this.HasRequired(m => m.ShippingAddress)
                .WithMany()
                .HasForeignKey(m => m.ShippingAddressId)
                .WillCascadeOnDelete(false);
        }
    }
}
