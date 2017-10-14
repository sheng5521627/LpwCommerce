using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// 
        /// </summary>
        public PluginDescriptor PluginDescriptor
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Install()
        {
            PluginManager.MarkPluginAsInstalled(PluginDescriptor.SystemName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Uninstall()
        {
            PluginManager.MarkPluginAsUnInstalled(PluginDescriptor.SystemName);
        }
    }
}
