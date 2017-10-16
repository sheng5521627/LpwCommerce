using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Customers
{
    /// <summary>
    /// Represents the customer name formatting enumeration
    /// </summary>
    public enum CustomerNameFormat
    {
        /// <summary>
        /// Show emails
        /// </summary>
        ShowEmails = 1,
        /// <summary>
        /// Show usernames
        /// </summary>
        ShowUsernames = 2,
        /// <summary>
        /// Show full names
        /// </summary>
        ShowFullNames = 3,
        /// <summary>
        /// Show first name
        /// </summary>
        ShowFirstName = 10
    }
}
