using Core.Domain.Polls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Polls
{
    public partial class PollAnswerMap : NopEntityTypeConfiguration<PollAnswer>
    {
        public PollAnswerMap()
        {
            this.ToTable("PollAnswer");
            this.HasKey(pa => pa.Id);
            this.Property(pa => pa.Name).IsRequired();

            this.HasRequired(m => m.Poll)
                .WithMany(m => m.PollAnswers)
                .HasForeignKey(m => m.PollId)
                .WillCascadeOnDelete(true);
        }
    }
}
