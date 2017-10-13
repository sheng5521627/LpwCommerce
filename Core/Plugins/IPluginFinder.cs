using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Plugins
{
    /// <summary>
    /// 插件查找器
    /// </summary>
    public interface IPluginFinder
    {
        /// <summary>
        /// 验证插件对应的商店是否可用
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        bool AuthenticateStore(PluginDescriptor pluginDescriptor, int storeId);

        /// <summary>
        /// 获取插件的分组信息
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetPluginGroups();

        /// <summary>
        /// 获取插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadMode">插件的加载模式</param>
        /// <param name="storeId">商店id</param>
        /// <param name="group">分组信息</param>
        /// <returns></returns>
        IEnumerable<T> GetPlugins<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, int storeId = 0, string group = null) where T : class, IPlugin;

        /// <summary>
        /// 获取插件描述
        /// </summary>
        /// <param name="loadMode"></param>
        /// <param name="storeId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, int storeId = 0, string group = null);

        /// <summary>
        /// 获取对应类型的插件信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadMode"></param>
        /// <param name="storeId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, int storeId = 0, string group = null) where T : class, IPlugin;

        /// <summary>
        /// 通过插件的系统名称获取插件描述
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        PluginDescriptor GetPluginDescriptorBySystemName(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly);

        /// <summary>
        /// 通过插件的系统名称获取对应类型的插件描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="systemName"></param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly) where T : class, IPlugin;

        /// <summary>
        /// 重新加载插件
        /// </summary>
        void ReloadPlugins();
    }
}
