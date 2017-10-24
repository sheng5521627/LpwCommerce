using Core.Domain.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Forums
{
    public partial class ForumMap : NopEntityTypeConfiguration<Forum>
    {
        public ForumMap()
        {
            this.ToTable("Forums_Forum");
            this.HasKey(f => f.Id);
            this.Property(f => f.Name).IsRequired().HasMaxLength(200);

            this.HasRequired(f => f.ForumGroup)
                .WithMany(fg => fg.Forums)
                .HasForeignKey(f => f.ForumGroupId);
        }
    }
}
