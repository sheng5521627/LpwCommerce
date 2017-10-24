using Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Shipping
{
    public partial class ShipmentItemMap : NopEntityTypeConfiguration<ShipmentItem>
    {
        public ShipmentItemMap()
        {
            this.ToTable("ShipmentItem");
            this.HasKey(si => si.Id);

            this.HasRequired(m => m.Shipment)
                .WithMany(m => m.ShipmentItems)
                .HasForeignKey(m => m.ShipmentId);
        }
    }
}
