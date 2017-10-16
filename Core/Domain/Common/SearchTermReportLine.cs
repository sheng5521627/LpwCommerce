﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Common
{
    /// <summary>
    /// Search term record (for statistics)
    /// </summary>
    public class SearchTermReportLine
    {
        /// <summary>
        /// Gets or sets the keyword
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets search count
        /// </summary>
        public int Count { get; set; }
    }
}
