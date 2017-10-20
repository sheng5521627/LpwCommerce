using Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Directory
{
    public partial class StateProvinceMap : NopEntityTypeConfiguration<StateProvince>
    {
        public StateProvinceMap()
        {
            this.ToTable("StateProvince");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(100);
            this.Property(m => m.Abbreviation).HasMaxLength(100);

            this.HasRequired(m => m.Country)
                .WithMany()
                .HasForeignKey(m => m.CountryId);
        }
    }
}
