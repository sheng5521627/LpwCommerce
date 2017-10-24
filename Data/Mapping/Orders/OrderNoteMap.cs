using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class OrderNoteMap : NopEntityTypeConfiguration<OrderNote>
    {
        public OrderNoteMap()
        {
            this.ToTable("OrderNote");
            this.HasKey(on => on.Id);
            this.Property(on => on.Note).IsRequired();

            this.HasRequired(m => m.Order)
                .WithMany(m => m.OrderNotes)
                .HasForeignKey(m => m.OrderId);
        }
    }
}
