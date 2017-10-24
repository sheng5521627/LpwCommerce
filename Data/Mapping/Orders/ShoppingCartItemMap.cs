using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class ShoppingCartItemMap : NopEntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            this.ToTable("ShoppingCartItem");
            this.HasKey(sci => sci.Id);

            this.Property(sci => sci.CustomerEnteredPrice).HasPrecision(18, 4);

            this.Ignore(sci => sci.ShoppingCartType);
            this.Ignore(sci => sci.IsFreeShipping);
            this.Ignore(sci => sci.IsShipEnabled);
            this.Ignore(sci => sci.AdditionalShippingCharge);
            this.Ignore(sci => sci.IsTaxExempt);

            this.HasRequired(m => m.Customer)
                .WithMany(m => m.ShoppingCartItems)
                .HasForeignKey(m => m.CustomerId);

            this.HasRequired(m => m.Product)
                .WithMany()
                .HasForeignKey(m => m.ProductId);
        }
    }
}