using Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Customers
{
    public partial class CustomerMap : NopEntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            this.ToTable("Customer");
            this.HasKey(m => m.Id);
            this.Property(m => m.Username).HasMaxLength(1000);
            this.Property(m => m.Email).HasMaxLength(1000);
            this.Property(m => m.SystemName).HasMaxLength(1000);

            this.Ignore(m => m.PasswordFormat);

            this.HasMany(m => m.CustomerRoles)
                .WithMany()
                .Map(m => m.ToTable("Customer_CustomerRole_Mapping"));

            this.HasMany(m => m.Addresses)
                .WithMany()
                .Map(m => m.ToTable("CustomerAddresses"));
        }
    }
}
