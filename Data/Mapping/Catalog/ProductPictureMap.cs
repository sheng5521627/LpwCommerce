using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class ProductPictureMap : NopEntityTypeConfiguration<ProductPicture>
    {
        public ProductPictureMap()
        {
            this.ToTable("Product_Picture_Mapping");
            this.HasKey(pp => pp.Id);

            this.HasRequired(m => m.Picture)
                .WithMany()
                .HasForeignKey(m => m.PictureId);

            this.HasRequired(m => m.Product)
                .WithMany(m => m.ProductPictures)
                .HasForeignKey(m => m.PictureId);
        }
    }
}
