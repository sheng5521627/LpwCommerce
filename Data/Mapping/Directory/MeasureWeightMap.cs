using Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Directory
{
    public partial class MeasureWeightMap : NopEntityTypeConfiguration<MeasureWeight>
    {
        public MeasureWeightMap()
        {
            this.ToTable("MeasureWeightMap");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(100);
            this.Property(m => m.SystemKeyword).IsRequired().HasMaxLength(100);
            this.Property(m => m.Ratio).HasPrecision(18, 8);
        }
    }
}
