﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Catalog
{
    /// <summary>
    /// Represents a download activation type
    /// </summary>
    public enum DownloadActivationType
    {
        /// <summary>
        /// When order is paid
        /// </summary>
        WhenOrderIsPaid = 0,

        /// <summary>
        /// Manually
        /// </summary>
        Manually = 10,
    }
}
