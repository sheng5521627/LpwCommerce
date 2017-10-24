using Core.Domain.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.News
{
    public partial class NewsCommentMap:NopEntityTypeConfiguration<NewsComment>
    {
        public NewsCommentMap()
        {
            this.ToTable("NewsComment");
            this.HasKey(m => m.Id);

            this.HasRequired(m => m.NewsItem)
                .WithMany(m => m.NewsComments)
                .HasForeignKey(m => m.NewsItemId);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId);
        }
    }
}
