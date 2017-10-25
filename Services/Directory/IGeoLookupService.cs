using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Directory
{
    /// <summary>
    /// GEO lookup service
    /// 地理位置查询
    /// </summary>
    public partial interface IGeoLookupService
    {
        /// <summary>
        /// Get country name
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>Country name</returns>
        string LookupCountryIsoCode(string ipAddress);

        /// <summary>
        /// Get country name
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>Country name</returns>
        string LookupCountryName(string ipAddress);
    }
}
