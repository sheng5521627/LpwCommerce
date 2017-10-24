using Core.Domain.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Blogs
{
    public partial class BlogPostMap:NopEntityTypeConfiguration<BlogPost>
    {
        public BlogPostMap()
        {
            this.ToTable("BlogPost");
            this.HasKey(m => m.Id);
            this.Property(m => m.Title).IsRequired();
            this.Property(m => m.Body).IsRequired();
            this.Property(m => m.MetaTitle).HasMaxLength(400);
            this.Property(m => m.MetaKeywords).HasMaxLength(400);

            this.HasRequired(m => m.Language)
                .WithMany()
                .HasForeignKey(m => m.LanguageId)
                .WillCascadeOnDelete(true);
        }
    }
}
