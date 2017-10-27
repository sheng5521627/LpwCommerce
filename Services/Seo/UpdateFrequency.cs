using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Seo
{
    /// <summary>
    /// Represents a sitemap update frequency
    /// </summary>
    public enum UpdateFrequency
    {
        /// <summary>
        /// Always
        /// </summary>
        Always,
        /// <summary>
        /// Hourly
        /// </summary>
        Hourly,
        /// <summary>
        /// Daily
        /// </summary>
        Daily,
        /// <summary>
        /// Weekly
        /// </summary>
        Weekly,
        /// <summary>
        /// Monthly
        /// </summary>
        Monthly,
        /// <summary>
        /// Yearly
        /// </summary>
        Yearly,
        /// <summary>
        /// Never
        /// </summary>
        Never
    }
}
