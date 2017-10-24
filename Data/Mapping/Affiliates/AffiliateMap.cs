using Core.Domain.Affiliates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Affiliates
{
    public partial class AffiliateMap : NopEntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            this.ToTable("Affiliate");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.Address)
                .WithMany()
                .HasForeignKey(m => m.AddressId)
                .WillCascadeOnDelete(false);
        }
    }
}
