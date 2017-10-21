using Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Customers
{
    public partial class CustomerAttributeMap : NopEntityTypeConfiguration<CustomerAttribute>
    {
        public CustomerAttributeMap()
        {
            this.ToTable("CustomerAttribute");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);

            this.Ignore(m => m.AttributeControlType);
        }
    }
}
