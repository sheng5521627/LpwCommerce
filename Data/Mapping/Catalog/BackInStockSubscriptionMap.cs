using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class BackInStockSubscriptionMap:NopEntityTypeConfiguration<BackInStockSubscription>
    {
        public BackInStockSubscriptionMap()
        {
            this.ToTable("BackInStockSubscription");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.Product)
                .WithMany()
                .HasForeignKey(m => m.ProductId)
                .WillCascadeOnDelete(true);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId)
                .WillCascadeOnDelete(true);
        }
    }
}
