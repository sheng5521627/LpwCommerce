using Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Vendors
{
    public partial class VendorNoteMap:NopEntityTypeConfiguration<VendorNote>
    {
        public VendorNoteMap()
        {
            this.ToTable("VendorNote");
            this.HasKey(m => m.Id);
            this.Property(m => m.Note).IsRequired();

            this.HasRequired(m => m.Vendor)
                .WithMany(m => m.VendorNotes)
                .HasForeignKey(m => m.VendorId);
        }
    }
}
