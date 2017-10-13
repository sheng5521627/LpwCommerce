using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Plugins
{
    /// <summary>
    /// 加载插件的方式
    /// </summary>
    public enum LoadPluginsMode
    {
        /// <summary>
        /// 加载所有的插件
        /// </summary>
        All = 0,
        /// <summary>
        /// 加载已安装的插件
        /// </summary>
        InstalledOnly = 1,
        /// <summary>
        /// 加载未安装的插件
        /// </summary>
        NotInstalledOnly = 2
    }
}
