using Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Localization
{
    public partial class LocalizedPropertyMap : NopEntityTypeConfiguration<LocalizedProperty>
    {
        public LocalizedPropertyMap()
        {
            this.ToTable("LocalizedProperty");
            this.HasKey(m => m.Id);
            this.Property(m => m.LocaleKeyGroup).IsRequired().HasMaxLength(400);
            this.Property(m => m.LocaleKey).IsRequired().HasMaxLength(400);
            this.Property(m => m.LocaleValue).IsRequired();

            this.HasRequired(m => m.Language)
                .WithMany()
                .HasForeignKey(m => m.LanguageId);
        }
    }
}
