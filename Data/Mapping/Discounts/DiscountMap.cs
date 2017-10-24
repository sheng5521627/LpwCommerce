using Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Discounts
{
    public partial class DiscountMap : NopEntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            this.ToTable("Discount");
            this.HasKey(d => d.Id);
            this.Property(d => d.Name).IsRequired().HasMaxLength(200);
            this.Property(d => d.CouponCode).HasMaxLength(100);
            this.Property(d => d.DiscountPercentage).HasPrecision(18, 4);
            this.Property(d => d.DiscountAmount).HasPrecision(18, 4);
            this.Property(d => d.MaximumDiscountAmount).HasPrecision(18, 4);

            this.Ignore(d => d.DiscountType);
            this.Ignore(d => d.DiscountLimitation);

            this.HasMany(m => m.DiscountRequirements)
                .WithRequired(m => m.Discount)
                .HasForeignKey(m => m.DiscountId);

            this.HasMany(m => m.AppliedToCategories)
                .WithMany(m => m.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToCategories"));

            this.HasMany(m => m.AppliedToManufacturers)
                .WithMany(m => m.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToManufacturers"));

            this.HasMany(m => m.AppliedToProducts)
                .WithMany(m => m.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToProducts"));
        }
    }
}
