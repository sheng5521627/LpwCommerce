﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Customers;

namespace Core.Plugins
{
    /// <summary>
    /// 插件查找器
    /// </summary>
    public class PluginFinder : IPluginFinder
    {
        #region Field

        private IList<PluginDescriptor> _plugins;
        private bool _arePluginsLoaded;

        #endregion

        /// <summary>
        /// 确保插件已全部加载
        /// </summary>
        protected virtual void EnsurePluginsAreLoaded()
        {
            if (!_arePluginsLoaded)
            {
                var foundPlugins = PluginManager.ReferencedPlugins.ToList();
                foundPlugins.Sort();
                _plugins = foundPlugins.ToList();
                _arePluginsLoaded = true;
            }
        }

        /// <summary>
        /// 判断插件的加载模式
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected virtual bool CheckLoadMode(PluginDescriptor pluginDescriptor, LoadPluginsMode mode)
        {
            if (pluginDescriptor == null)
                throw new ArgumentException("pluginDescriptor");

            switch (mode)
            {
                case LoadPluginsMode.All:
                    return true;
                case LoadPluginsMode.InstalledOnly:
                    return pluginDescriptor.Installed;
                case LoadPluginsMode.NotInstalledOnly:
                    return !pluginDescriptor.Installed;
                default:
                    throw new Exception("没有该加载模式");
            }
        }

        /// <summary>
        /// 判断插件的分组
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        protected virtual bool CheckGroup(PluginDescriptor pluginDescriptor, string group)
        {
            if (pluginDescriptor == null)
                throw new ArgumentException("pluginDescriptor");

            if (string.IsNullOrEmpty(group))
                return true;

            return group.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 判断插件所属的商店
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool AuthenticateStore(PluginDescriptor pluginDescriptor, int storeId)
        {
            if (pluginDescriptor == null)
                throw new ArgumentException("pluginDescriptor");

            if (storeId == 0)
                return true;

            if (pluginDescriptor.LimitedToStores.Count == 0)
                return true;

            return pluginDescriptor.LimitedToStores.Contains(storeId);
        }

        /// <summary>
        /// 获取所有插件的分组
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetPluginGroups()
        {
            return GetPluginDescriptors(LoadPluginsMode.All).Select(x => x.Group).Distinct().OrderBy(x => x);
        }                            
        
        /// <summary>
        /// 获取对应的插件描述
        /// </summary>
        /// <param name="loadMode"></param>
        /// <param name="storeId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,int storeId = 0,string group = null)
        {
            EnsurePluginsAreLoaded();
            return _plugins.Where(x => x.Installed == CheckLoadMode(x, loadMode) && AuthenticateStore(x, storeId) && CheckGroup(x, group));
        }

        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, int storeId = 0, string group = null) where T : class, IPlugin
        {
            return GetPluginDescriptors(loadMode, storeId, group).Where(x => typeof(T).IsAssignableFrom(x.PluginType));
        }

        public virtual IEnumerable<T> GetPlugins<T>(LoadPluginsMode loadMode, int storeId, string group) where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(loadMode, storeId, group).Select(x => x.Instance<T>());
        }

        public PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, LoadPluginsMode loadMode) where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(loadMode).FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        public PluginDescriptor GetPluginDescriptorBySystemName(string systemName, LoadPluginsMode loadMode)
        {
            return GetPluginDescriptors(loadMode).FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual void ReloadPlugins()
        {
            _arePluginsLoaded = false;
            EnsurePluginsAreLoaded();
        }

        /// <summary>
        /// Gets plugins
        /// </summary>
        /// <typeparam name="T">The type of plugins to get.</typeparam>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugins</returns>
        public virtual IEnumerable<T> GetPluginsNews<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where T : class, IPlugin
        {
            return GetPluginDescriptorsByCustomer<T>(loadMode, customer, storeId, group).Select(p => p.Instance<T>());
        }
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptorsByCustomer<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null)
            where T : class, IPlugin
        {
            return GetPluginDescriptorsByCustomer(loadMode, customer, storeId, group)
                .Where(p => typeof(T).IsAssignableFrom(p.PluginType));
        }
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptorsByCustomer(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null)
        {
            //ensure plugins are loaded
            EnsurePluginsAreLoaded();

            return _plugins.Where(p => CheckLoadMode(p, loadMode) && AuthorizedForUser(p, customer) && AuthenticateStore(p, storeId) && CheckGroup(p, group));
        }
        /// <summary>
        /// Check that plugin is available for the specified customer
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="customer">Customer</param>
        /// <returns>True if authorized; otherwise, false</returns>
        public virtual bool AuthorizedForUser(PluginDescriptor pluginDescriptor, Customer customer)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException("pluginDescriptor");

            if (customer == null || !pluginDescriptor.LimitedToCustomerRoles.Any())
                return true;

            var customerRoleIds = customer.CustomerRoles.Where(role => role.Active).Select(role => role.Id);

            return pluginDescriptor.LimitedToCustomerRoles.Intersect(customerRoleIds).Any();
        }

    }
}
