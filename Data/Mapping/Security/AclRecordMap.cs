using Core.Domain.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Security
{
    public partial class AclRecordMap : NopEntityTypeConfiguration<AclRecord>
    {
        public AclRecordMap()
        {
            this.ToTable("AclRecord");
            this.HasKey(ar => ar.Id);

            this.Property(ar => ar.EntityName).IsRequired().HasMaxLength(400);

            this.HasRequired(m => m.CustomerRole)
                .WithMany()
                .HasForeignKey(m => m.CustomerRoleId)
                .WillCascadeOnDelete(true);
        }
    }
}
