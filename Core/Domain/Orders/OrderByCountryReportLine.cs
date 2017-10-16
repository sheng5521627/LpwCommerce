using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Orders
{
    /// <summary>
    /// Represents an "order by country" report line
    /// </summary>
    public partial class OrderByCountryReportLine
    {
        /// <summary>
        /// Country identifier; null for unknow country
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the number of orders
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Gets or sets the order total summary
        /// </summary>
        public decimal SumOrders { get; set; }
    }
}
