using Core.Configuration;
using Core.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Configuration
{
    public partial class SettingMap : NopEntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("Setting");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(200);
            this.Property(m => m.Value).IsRequired().HasMaxLength(2000);
        }
    }
}
