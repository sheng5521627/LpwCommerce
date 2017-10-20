using Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Directory
{
    public partial class CountryMap : NopEntityTypeConfiguration<Country>
    {
        public CountryMap()
        {
            this.ToTable("Country");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(100);
            this.Property(m => m.TwoLetterIsoCode).HasMaxLength(2);
            this.Property(m => m.ThreeLetterIsoCode).HasMaxLength(3);
        }
    }
}
