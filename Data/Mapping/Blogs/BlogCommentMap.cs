using Core.Domain.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Blogs
{
    public partial class BlogCommentMap:NopEntityTypeConfiguration<BlogComment>
    {
        public BlogCommentMap()
        {
            this.ToTable("BlogComment");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.BlogPost)
                .WithMany(m => m.BlogComments)
                .HasForeignKey(m => m.BlogPostId);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId);
        }
    }
}
