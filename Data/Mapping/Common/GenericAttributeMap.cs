using Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Common
{
    public partial class GenericAttributeMap:NopEntityTypeConfiguration<GenericAttribute>
    {
        public GenericAttributeMap()
        {
            this.ToTable("GenericAttribute");
            this.HasKey(m => m.Id);
            this.Property(m => m.KeyGroup).IsRequired().HasMaxLength(400);
            this.Property(m => m.Key).IsRequired().HasMaxLength(400);
            this.Property(m => m.Value).IsRequired();
        }
    }
}
