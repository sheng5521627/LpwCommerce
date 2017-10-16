using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Tax
{
    /// <summary>
    /// Represents the tax display type enumeration
    /// </summary>
    public enum TaxDisplayType
    {
        /// <summary>
        /// Including tax
        /// </summary>
        IncludingTax = 0,
        /// <summary>
        /// Excluding tax
        /// </summary>
        ExcludingTax = 10,
    }
}
