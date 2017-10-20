using Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Localization
{
    public partial class LocaleStringResourceMap : NopEntityTypeConfiguration<LocaleStringResource>
    {
        public LocaleStringResourceMap()
        {
            this.ToTable("LocaleStringResource");
            this.HasKey(m => m.Id);
            this.Property(m => m.ResourceName).IsRequired().HasMaxLength(200);
            this.Property(m => m.ResourceValue).IsRequired();

            this.HasRequired(m => m.Language)
                .WithMany(m => m.LocaleStringResources)
                .HasForeignKey(m => m.LanguageId);
        }
    }
}
