using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Plugins
{
    /// <summary>
    /// 插件描述
    /// </summary>
    public class PluginDescriptor : IComparable<PluginDescriptor>
    {
        /// <summary>
        /// 插件文件名称
        /// </summary>
        public virtual string PluginFileName { get; set; }

        /// <summary>
        /// 插件类型
        /// </summary>
        public virtual Type PluginType { get; set; }

        /// <summary>
        /// 插件所属的程序集
        /// </summary>
        public virtual Assembly RefrencedAssembly { get; internal set; }

        /// <summary>
        /// 插件所属的文件
        /// </summary>
        public virtual FileInfo OriginalAssemblyFile { get; internal set; }

        /// <summary>
        /// 插件分组
        /// </summary>
        public virtual string Group { get; set; }

        /// <summary>
        /// 插件友好名称
        /// </summary>
        public virtual string FriendlyName { get; set; }

        /// <summary>
        /// 插件系统名称
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// 插件支持的系统版本
        /// </summary>
        public virtual IList<string> SupportedVersions { get; set; }

        /// <summary>
        /// 插件作者
        /// </summary>
        public virtual string Author { get; set; }

        /// <summary>
        /// 插件的排序
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// 插件支持的商店
        /// </summary>
        public virtual IList<int> LimitedToStores { get; set; }

        /// <summary>
        /// 插件是否安装
        /// </summary>
        public virtual bool Installed { get; set; }

        public PluginDescriptor()
        {
            this.SupportedVersions = new List<string>();
            this.LimitedToStores = new List<int>();
        }

        public PluginDescriptor(Assembly referenceAssembly, FileInfo originalAssemblyFile, Type pluginType)
            : this()
        {
            this.RefrencedAssembly = referenceAssembly;
            this.OriginalAssemblyFile = originalAssemblyFile;
            this.PluginType = pluginType;
        }

        /// <summary>
        /// 获取插件实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Instance<T>() where T : class, IPlugin
        {
            object instance;
            if (!EngineContext.Current.ContainnerManager.TryResolve(PluginType, null, out instance))
            {
                instance = EngineContext.Current.ContainnerManager.ResolveUnregistered(PluginType);
            }

            var typedInstance = instance as T;
            if (typedInstance != null)
            {
                typedInstance.PluginDescriptor = this;
            }
            return typedInstance;
        }

        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }

        /// <summary>
        /// 获取插件的实例
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PluginDescriptor other)
        {
            if (DisplayOrder != other.DisplayOrder)
            {
                return DisplayOrder.CompareTo(other.DisplayOrder);
            }
            return FriendlyName.CompareTo(other.FriendlyName);
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PluginDescriptor;
            return other != null && SystemName != null && SystemName.Equals(other.SystemName);
        }

        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }
    }
}
