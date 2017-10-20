using Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Shipping
{
    public partial class ShippingMethodMap:NopEntityTypeConfiguration<ShippingMethod>
    {
        public ShippingMethodMap()
        {
            this.ToTable("ShippingMethod");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);

            this.HasMany(m => m.RestrictedCountries)
                .WithMany(m => m.RestrictedShippingMethods)
                .Map(m => m.ToTable("ShippingMethodRestrictions"));
        }
    }
}
