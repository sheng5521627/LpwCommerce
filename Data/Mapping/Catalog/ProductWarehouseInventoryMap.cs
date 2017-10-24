using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductWarehouseInventoryMap : NopEntityTypeConfiguration<ProductWarehouseInventory>
    {
        public ProductWarehouseInventoryMap()
        {
            this.ToTable("ProductWarehouseInventory");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductWarehouseInventory)
                .HasForeignKey(m => m.ProductId)
                .WillCascadeOnDelete(true);

            this.HasRequired(m => m.Warehouse)
                .WithMany()
                .HasForeignKey(m => m.WarehouseId)
                .WillCascadeOnDelete(true);
        }
    }
}
