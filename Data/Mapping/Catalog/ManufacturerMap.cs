using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ManufacturerMap : NopEntityTypeConfiguration<Manufacturer>
    {
        public ManufacturerMap()
        {
            this.ToTable("Manufacturer");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);
            this.Property(m => m.MetaKeywords).HasMaxLength(400);
            this.Property(m => m.MetaTitle).HasMaxLength(400);
            this.Property(m => m.PriceRanges).HasMaxLength(400);
            this.Property(m => m.PageSizeOptions).HasMaxLength(200);
        }
    }
}
