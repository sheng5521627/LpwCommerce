using Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Customers
{
    public partial class CustomerAttributeValueMap:NopEntityTypeConfiguration<CustomerAttributeValue>
    {
        public CustomerAttributeValueMap()
        {
            this.ToTable("CustomerAttributeValue");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);

            this.HasRequired(m => m.CustomerAttribute)
                .WithMany(m => m.CustomerAttributeValues)
                .HasForeignKey(m => m.CustomerAttributeId);
        }
    }
}
