using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Authentication.External
{
    /// <summary>
    /// External provider authorizer
    /// </summary>
    public partial interface IExternalProviderAuthorizer
    {
        /// <summary>
        /// Authorize response
        /// </summary>
        /// <param name="returnUrl">Return URL</param>
        /// <param name="verifyResponse">true - Verify response;false - request authentication;null - determine automatically</param>
        /// <returns>Authorize state</returns>
        AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null);
    }
}
