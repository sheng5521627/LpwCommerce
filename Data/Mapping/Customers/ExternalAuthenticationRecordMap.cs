using Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Customers
{
    public partial class ExternalAuthenticationRecordMap : NopEntityTypeConfiguration<ExternalAuthenticationRecord>
    {
        public ExternalAuthenticationRecordMap()
        {
            this.ToTable("ExternalAuthenticationRecord");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.Customer)
                .WithMany(m => m.ExternalAuthenticationRecords)
                .HasForeignKey(m => m.CustomerId);
        }
    }
}
