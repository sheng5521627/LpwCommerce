using Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Shipping
{
    public partial class ShipmentMap : NopEntityTypeConfiguration<Shipment>
    {
        public ShipmentMap()
        {
            this.ToTable("Shipment");
            this.HasKey(s => s.Id);

            this.Property(s => s.TotalWeight).HasPrecision(18, 4);

            this.HasRequired(m => m.Order)
                .WithMany(m => m.Shipments)
                .HasForeignKey(m => m.OrderId);
        }
    }
}