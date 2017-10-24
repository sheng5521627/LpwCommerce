using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductManufacturerMap : NopEntityTypeConfiguration<ProductManufacturer>
    {
        public ProductManufacturerMap()
        {
            this.ToTable("Product_Manufacturer_Mapping");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.Manufacturer)
                .WithMany()
                .HasForeignKey(m => m.ManufacturerId);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductManufacturers)
                .HasForeignKey(m => m.ProductId);
        }
    }
}
