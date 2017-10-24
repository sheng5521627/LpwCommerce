using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class RecurringPaymentMap : NopEntityTypeConfiguration<RecurringPayment>
    {
        public RecurringPaymentMap()
        {
            this.ToTable("RecurringPayment");
            this.HasKey(rp => rp.Id);

            this.Ignore(rp => rp.NextPaymentDate);
            this.Ignore(rp => rp.CyclesRemaining);
            this.Ignore(rp => rp.CyclePeriod);

            this.HasRequired(m => m.InitialOrder)
                .WithMany()
                .HasForeignKey(m => m.InitialOrderId)
                .WillCascadeOnDelete(false);
        }
    }
}