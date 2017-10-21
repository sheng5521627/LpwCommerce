using Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Common
{
    public partial class AddressMap:NopEntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            this.ToTable("Address");
            this.HasKey(m => m.Id);

            this.HasOptional(m => m.Country)
                .WithMany()
                .HasForeignKey(m => m.CountryId)
                .WillCascadeOnDelete(false);

            this.HasOptional(m => m.StateProvince)
                .WithMany()
                .HasForeignKey(m => m.StateProvinceId)
                .WillCascadeOnDelete(false);
        }
    }
}
