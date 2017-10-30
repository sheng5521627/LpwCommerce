using Core.Domain.Customers;
using Services.Authentication.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Authentication.External
{
    public static class OpenAuthenticationExtensions
    {
        public static bool IsMethodActive(this IExternalAuthenticationMethod method,
            ExternalAuthenticationSettings settings)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (settings == null)
                throw new ArgumentNullException("settings");

            if (settings.ActiveAuthenticationMethodSystemNames == null)
                return false;
            foreach (string activeMethodSystemName in settings.ActiveAuthenticationMethodSystemNames)
                if (method.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
    }
}
