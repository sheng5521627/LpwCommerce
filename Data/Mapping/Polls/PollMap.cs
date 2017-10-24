using Core.Domain.Polls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Polls
{
    public partial class PollMap : NopEntityTypeConfiguration<Poll>
    {
        public PollMap()
        {
            this.ToTable("Poll");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired();

            this.HasRequired(m => m.Language)
                .WithMany()
                .HasForeignKey(m => m.LanguageId)
                .WillCascadeOnDelete(true);
        }
    }
}
