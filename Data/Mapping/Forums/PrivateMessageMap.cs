using Core.Domain.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Forums
{
    public partial class PrivateMessageMap : NopEntityTypeConfiguration<PrivateMessage>
    {
        public PrivateMessageMap()
        {
            this.ToTable("Forums_PrivateMessage");
            this.HasKey(pm => pm.Id);
            this.Property(pm => pm.Subject).IsRequired().HasMaxLength(450);
            this.Property(pm => pm.Text).IsRequired();

            this.HasRequired(m => m.FromCustomer)
                .WithMany()
                .HasForeignKey(m => m.FromCustomerId)
                .WillCascadeOnDelete(false);

            this.HasRequired(m => m.ToCustomer)
                .WithMany()
                .HasForeignKey(m => m.ToCustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}
