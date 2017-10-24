using Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Discounts
{
    public partial class DiscountUsageHistoryMap : NopEntityTypeConfiguration<DiscountUsageHistory>
    {
        public DiscountUsageHistoryMap()
        {
            this.ToTable("DiscountUsageHistory");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.Discount)
                .WithMany()
                .HasForeignKey(m => m.DiscountId);

            this.HasRequired(m => m.Order)
                .WithMany(m => m.DiscountUsageHistory)
                .HasForeignKey(m => m.OrderId);
        }
    }
}
