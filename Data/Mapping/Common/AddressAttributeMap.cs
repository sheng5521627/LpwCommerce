using Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Common
{
    public partial class AddressAttributeMap : NopEntityTypeConfiguration<AddressAttribute>
    {
        public AddressAttributeMap()
        {
            this.ToTable("AddressAttribute");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);

            this.Ignore(m => m.AttributeControlType);
        }
    }
}
