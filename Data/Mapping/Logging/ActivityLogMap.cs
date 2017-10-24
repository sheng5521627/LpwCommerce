using Core.Domain.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Logging
{
    public partial class ActivityLogMap : NopEntityTypeConfiguration<ActivityLog>
    {
        public ActivityLogMap()
        {
            this.ToTable("ActivityLog");
            this.HasKey(al => al.Id);
            this.Property(al => al.Comment).IsRequired();

            this.HasRequired(m => m.ActivityLogType)
                .WithMany()
                .HasForeignKey(m => m.ActivityLogTypeId);

            this.HasRequired(m => m.Customer)
                .WithMany()
                .HasForeignKey(m => m.CustomerId);
        }
    }
}
