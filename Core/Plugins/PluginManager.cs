using Core.ComponentModel;
using Core.Plugins;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]
namespace Core.Plugins
{
    /// <summary>
    /// 插件管理器
    /// </summary>
    public class PluginManager
    {
        #region Const

        /// <summary>
        /// 已安装插件的文件
        /// </summary>
        private const string InstalledPluginsFilePath = "~/App_Data/InstalledPlugins.txt";

        /// <summary>
        /// 插件所在的目录
        /// </summary>
        private const string PluginsPath = "~/Plugins";

        /// <summary>
        /// 插件副本
        /// </summary>
        private const string ShadowCopyPath = "~/Plugins/bin";

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        private static DirectoryInfo _shadowCopyFolder;

        private static bool _clearShadowDirectoryOnStartup;

        #endregion

        /// <summary>
        /// 所有引用的插件
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        /// 所有不匹配的插件
        /// </summary>
        public static IEnumerable<string> IncompatiblePlugins { get; set; }

        /// <summary>
        /// 应用程序启动前先执行该代码
        /// </summary>
        public static void Initialize()
        {
            using (new WriteLockDisposable(Locker))
            {
                var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(PluginsPath));
                _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(ShadowCopyPath));

                var referencedPlugins = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();

                _clearShadowDirectoryOnStartup = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]) &&
                    Convert.ToBoolean(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]);

                try
                {
                    var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());

                    Directory.CreateDirectory(pluginFolder.FullName);
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

                    var binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                    if (_clearShadowDirectoryOnStartup)
                    {
                        foreach (var f in binFiles)
                        {
                            try
                            {
                                File.Delete(f.FullName);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginFolder))
                    {
                        var descriptionFile = dfd.Key;
                        var pluginDescriptor = dfd.Value;

                        if (!pluginDescriptor.SupportedVersions.Contains(NopVersion.CurrentVersion, StringComparer.InvariantCultureIgnoreCase))
                        {
                            incompatiblePlugins.Add(pluginDescriptor.SystemName);
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
                            throw new NopException(string.Format("插件{0}没有系统名称", descriptionFile.FullName));
                        if (referencedPlugins.Contains(pluginDescriptor))
                            throw new NopException(string.Format("系统名称{0}已经定义过", pluginDescriptor.SystemName));

                        pluginDescriptor.Installed = installedPluginSystemNames.FirstOrDefault(x => x.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase)) != null;

                        try
                        {
                            if (descriptionFile.Directory == null)
                                throw new NopException("该目录不能被解析.");
                            //get list of all DLLs in plugins (not in bin!)
                            var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => IsPackagePluginFoler(x.Directory)).ToList();

                            var mainPluginFile = pluginFiles.FirstOrDefault(x => x.Name.Equals(pluginDescriptor.PluginFileName, StringComparison.InvariantCultureIgnoreCase));
                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

                            //加载主dll
                            pluginDescriptor.RefrencedAssembly = PerformFileDeploy(mainPluginFile);

                            //加载所有dll
                            foreach (var plugin in pluginFiles
                                .Where(x => !x.Name.Equals(pluginDescriptor.PluginFileName, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !IsAlreadyLoaded(x)))
                            {
                                PerformFileDeploy(plugin);
                            }

                            //初始化插件类型(一个程序集只有一个Plugin)
                            foreach (var type in pluginDescriptor.RefrencedAssembly.GetTypes())
                            {
                                if (typeof(IPlugin).IsAssignableFrom(type))
                                {
                                    if (!type.IsInterface && type.IsClass && !type.IsAbstract)
                                    {
                                        pluginDescriptor.PluginType = type;
                                        break;
                                    }
                                }
                            }
                            referencedPlugins.Add(pluginDescriptor);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            var msg = string.Format("Plugin '{0}'", pluginDescriptor.FriendlyName);
                            foreach (var e in ex.LoaderExceptions)
                                msg += e.Message + Environment.NewLine;
                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                        catch (Exception ex)
                        {
                            var msg = string.Format("Plugin '{0}'. {1}", pluginDescriptor.FriendlyName, ex.Message);
                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;
                    var fail = new Exception(msg, ex);
                    throw fail;
                }
                ReferencedPlugins = referencedPlugins;
                IncompatiblePlugins = incompatiblePlugins;
            }
        }

        /// <summary>
        /// 标记插件已安装
        /// </summary>
        /// <param name="systemName"></param>
        public static void MarkPluginAsInstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentException("systemName");

            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {

                }
            var installedPlginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());

            bool alreadyMarkedAsInstalled = installedPlginSystemNames.FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (!alreadyMarkedAsInstalled)
            {
                installedPlginSystemNames.Add(systemName);
            }
            PluginFileParser.SaveInstalledPluginsFile(installedPlginSystemNames, filePath);
        }

        /// <summary>
        /// 标记插件已卸载
        /// </summary>
        /// <param name="systemName"></param>
        public static void MarkPluginAsUnInstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentException("systemName");

            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {

                }
            var installedPlginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());

            bool alreadyMarkedAsInstalled = installedPlginSystemNames.FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (alreadyMarkedAsInstalled)
            {
                installedPlginSystemNames.Remove(systemName);
            }
            PluginFileParser.SaveInstalledPluginsFile(installedPlginSystemNames, filePath);
        }

        /// <summary>
        /// 标记所有的插件已卸载
        /// </summary>
        public static void MarkAllPluginsAsUnInstalled()
        {
            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// 判断程序集是否已加载
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private static bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            try
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (fileNameWithoutExt == null)
                    throw new Exception("不存在该文件");
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string assemblyName = assembly.FullName.Split(new[] { ',' }).FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// 获取已安装插件的文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetInstalledPluginsFilePath()
        {
            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            return filePath;
        }

        /// <summary>
        /// 获取插件的描述信息
        /// </summary>
        /// <param name="pluginFolder"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(DirectoryInfo pluginFolder)
        {
            if (pluginFolder == null)
                throw new ArgumentException("pluginFolder");

            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();

            foreach (var descriptionFile in pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories))
            {
                if (!IsPackagePluginFoler(descriptionFile.Directory))
                    continue;

                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);
                result.Add(new KeyValuePair<FileInfo, Plugins.PluginDescriptor>(descriptionFile, pluginDescriptor));
            }

            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
            return result;
        }

        /// <summary>
        /// 判断目录是否所属插件目录下
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool IsPackagePluginFoler(DirectoryInfo folder)
        {
            if (folder == null)
                return false;
            if (folder.Parent == null)
                return false;
            if (!folder.Parent.Name.Equals("Plugins", StringComparison.InvariantCultureIgnoreCase))
                return false;
            return true;
        }

        /// <summary>
        /// 获取插件程序集
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        private static Assembly PerformFileDeploy(FileInfo plugin)
        {
            if (plugin.Directory.Parent == null)
            {
                throw new InvalidOperationException("The plugin directory for the " + plugin.Name + " file exists in a folder outside of the allowed nopCommerce folder heirarchy");
            }

            FileInfo shadowCopiedPlug;

            if (CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            {
                var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
                shadowCopiedPlug = InitializeMediumTrust(plugin, shadowCopyPlugFolder);
            }
            else
            {
                var directory = AppDomain.CurrentDomain.DynamicDirectory;
                shadowCopiedPlug = InitializeFullTrust(plugin, new DirectoryInfo(directory));
            }

            var shadowCopidAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));
            BuildManager.AddReferencedAssembly(shadowCopidAssembly);
            return shadowCopidAssembly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeFullTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeMediumTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            bool shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName + plug.Name));

            if (shadowCopiedPlug.Exists)
            {
                var areFilesIdenticial = shadowCopiedPlug.CreationTimeUtc.Ticks > plug.CreationTimeUtc.Ticks;
                if (areFilesIdenticial)
                    shouldCopy = false;
                else
                {
                    shouldCopy = true;
                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (shouldCopy)
            {
                try
                {
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");

                    try
                    {
                        var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlug.FullName, oldFile);
                    }
                    catch (IOException exc)
                    {
                        throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                    }

                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
            }
            return shadowCopiedPlug;
        }
    }
}
