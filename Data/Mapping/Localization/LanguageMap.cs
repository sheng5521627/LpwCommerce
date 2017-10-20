using Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Localization
{
    public partial class LanguageMap : NopEntityTypeConfiguration<Language>
    {
        public LanguageMap()
        {
            this.ToTable("Language");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(100);
            this.Property(m => m.LanguageCulture).IsRequired().HasMaxLength(20);
            this.Property(m => m.UniqueSeoCode).HasMaxLength(2);
            this.Property(m => m.FlagImageFileName).HasMaxLength(50);
        }
    }
}
