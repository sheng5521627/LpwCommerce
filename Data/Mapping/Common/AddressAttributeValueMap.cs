using Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Common
{
    public partial class AddressAttributeValueMap:NopEntityTypeConfiguration<AddressAttributeValue>
    {
        public AddressAttributeValueMap()
        {
            this.ToTable("AddressAttributeValue");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);

            this.HasRequired(m => m.AddressAttribute)
                .WithMany(m => m.AddressAttributeValues)
                .HasForeignKey(m => m.AddressAttributeId);
        }
    }
}
