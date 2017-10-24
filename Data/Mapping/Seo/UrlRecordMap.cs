﻿using Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping.Seo
{
    public partial class UrlRecordMap : NopEntityTypeConfiguration<UrlRecord>
    {
        public UrlRecordMap()
        {
            this.ToTable("UrlRecord");
            this.HasKey(lp => lp.Id);

            this.Property(lp => lp.EntityName).IsRequired().HasMaxLength(400);
            this.Property(lp => lp.Slug).IsRequired().HasMaxLength(400);
        }
    }
}
