using Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Customers
{
    public class RewardPointsHistoryMap:NopEntityTypeConfiguration<RewardPointsHistory>
    {
        public RewardPointsHistoryMap()
        {
            this.ToTable("RewardPointsHistory");
            this.HasKey(m => m.Id);

            this.Property(m => m.UsedAmount).HasPrecision(18, 4);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId);

            this.HasOptional(m => m.UsedWithOrder)
                .WithOptionalDependent(m => m.RedeemedRewardPointsEntry)
                .WillCascadeOnDelete(false);
        }
    }
}
