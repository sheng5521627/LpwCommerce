using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Catalog
{
    public partial class SpecificationAttributeOptionMap : NopEntityTypeConfiguration<SpecificationAttributeOption>
    {
        public SpecificationAttributeOptionMap()
        {
            this.ToTable("SpecificationAttributeOption");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired();

            this.HasRequired(m => m.SpecificationAttribute)
                .WithMany(m => m.SpecificationAttributeOptions)
                .HasForeignKey(m => m.SpecificationAttributeId);
        }
    }
}
