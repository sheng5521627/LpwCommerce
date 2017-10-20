using Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Directory
{
    public partial class CurrencyMap : NopEntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            this.ToTable("Currency");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);
            this.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(5);
            this.Property(c => c.DisplayLocale).HasMaxLength(50);
            this.Property(c => c.CustomFormatting).HasMaxLength(50);
            this.Property(c => c.Rate).HasPrecision(18, 4);
        }
    }
}
