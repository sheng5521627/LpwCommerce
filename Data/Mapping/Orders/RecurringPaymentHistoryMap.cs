using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Orders
{
    public partial class RecurringPaymentHistoryMap : NopEntityTypeConfiguration<RecurringPaymentHistory>
    {
        public RecurringPaymentHistoryMap()
        {
            this.ToTable("RecurringPaymentHistory");
            this.HasKey(rph => rph.Id);

            this.HasRequired(m => m.RecurringPayment)
                .WithMany(m => m.RecurringPaymentHistory)
                .HasForeignKey(m => m.RecurringPaymentId);

            //entity framework issue if we have navigation property to 'Order'
            //1. save recurring payment with an order
            //2. save recurring payment history with an order
            //3. update associated order => exception is thrown
        }
    }
}
