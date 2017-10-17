using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Infrastructure
{
    /// <summary>
    /// Default machine name provider
    /// </summary>
    public class DefaultMachineNameProvider : IMachineNameProvider
    {
        /// <summary>
        /// Returns the name of the machine (instance) running the application.
        /// </summary>
        public string GetMachineName()
        {
            return System.Environment.MachineName;
        }
    }
}
