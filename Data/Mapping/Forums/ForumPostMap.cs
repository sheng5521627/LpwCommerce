using Core.Domain.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Forums
{
    public partial class ForumPostMap : NopEntityTypeConfiguration<ForumPost>
    {
        public ForumPostMap()
        {
            this.ToTable("Forums_Post");
            this.HasKey(fp => fp.Id);
            this.Property(fp => fp.Text).IsRequired();
            this.Property(fp => fp.IPAddress).HasMaxLength(100);

            this.HasRequired(m => m.ForumTopic)
                .WithMany()
                .HasForeignKey(m => m.TopicId);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}
