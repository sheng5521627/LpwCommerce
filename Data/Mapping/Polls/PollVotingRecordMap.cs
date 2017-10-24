using Core.Domain.Polls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Polls
{
    public partial class PollVotingRecordMap : NopEntityTypeConfiguration<PollVotingRecord>
    {
        public PollVotingRecordMap()
        {
            this.ToTable("PollVotingRecord");
            this.HasKey(pr => pr.Id);

            this.HasRequired(m => m.PollAnswer)
                .WithMany(m => m.PollVotingRecords)
                .HasForeignKey(m => m.PollAnswerId);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId);
        }
    }
}
