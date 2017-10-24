using Core.Domain.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.News
{
    public partial class NewsItemMap : NopEntityTypeConfiguration<NewsItem>
    {
        public NewsItemMap()
        {
            this.ToTable("News");
            this.HasKey(ni => ni.Id);
            this.Property(ni => ni.Title).IsRequired();
            this.Property(ni => ni.Short).IsRequired();
            this.Property(ni => ni.Full).IsRequired();
            this.Property(ni => ni.MetaKeywords).HasMaxLength(400);
            this.Property(ni => ni.MetaTitle).HasMaxLength(400);

            this.HasRequired(m => m.Language)
                .WithMany()
                .HasForeignKey(m => m.LanguageId)
                .WillCascadeOnDelete(true);
        }
    }
}
