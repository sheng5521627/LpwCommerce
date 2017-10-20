using Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Stores
{
    public partial class StoreMappingMap : NopEntityTypeConfiguration<StoreMapping>
    {
        public StoreMappingMap()
        {
            this.ToTable("StoreMapping");
            this.HasKey(m => m.Id);
            this.Property(m => m.EntityName).IsRequired().HasMaxLength(400);

            this.HasRequired(m => m.Store)
                .WithMany()
                .HasForeignKey(m => m.StoreId)
                .WillCascadeOnDelete(true);
        }
    }
}
