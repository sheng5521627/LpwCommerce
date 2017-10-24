using Core.Domain.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Forums
{
    public partial class ForumTopicMap : NopEntityTypeConfiguration<ForumTopic>
    {
        public ForumTopicMap()
        {
            this.ToTable("Forums_Topic");
            this.HasKey(ft => ft.Id);
            this.Property(ft => ft.Subject).IsRequired().HasMaxLength(450);
            this.Ignore(ft => ft.ForumTopicType);

            this.HasRequired(m => m.Forum)
                .WithMany()
                .HasForeignKey(m => m.ForumId);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}
