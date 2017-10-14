using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Plugins
{
    /// <summary>
    /// 插件文件的转换类
    /// </summary>
    public static class PluginFileParser
    {
        /// <summary>
        /// 解析安装的插件文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IList<string> ParseInstalledPluginsFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<string>();

            var text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            var lines = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(str))
                        continue;
                    lines.Add(str);
                }
            }
            return lines;
        }

        /// <summary>
        /// 保存已安装的插件名
        /// </summary>
        /// <param name="pluginsSystemNames"></param>
        /// <param name="filePath"></param>
        public static void SaveInstalledPluginsFile(IList<string> pluginsSystemNames, string filePath)
        {
            string result = "";
            foreach (var name in pluginsSystemNames)
                result += string.Format("{0}{1}", name, Environment.NewLine);
            File.WriteAllText(filePath, result);
        }

        /// <summary>
        /// 解析插件描述文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static PluginDescriptor ParsePluginDescriptionFile(string filePath)
        {
            var descriptor = new PluginDescriptor();
            var text = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(text))
                return descriptor;

            var settings = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(str))
                        continue;
                    settings.Add(str.Trim());
                }
            }

            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(":");
                if (separatorIndex == -1)
                    continue;

                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex + 1).Trim();

                switch (key)
                {
                    case "Group":
                        descriptor.Group = value;
                        break;
                    case "FriendlyName":
                        descriptor.FriendlyName = value;
                        break;
                    case "SystemName":
                        descriptor.SystemName = value;
                        break;
                    case "Version":
                        descriptor.Version = value;
                        break;
                    case "SupportedVersions":
                        descriptor.SupportedVersions = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                        break;
                    case "Author":
                        descriptor.Author = value;
                        break;
                    case "DisplayOrder":
                        {
                            int displayOrder;
                            int.TryParse(value, out displayOrder);
                            descriptor.DisplayOrder = displayOrder;
                        }
                        break;
                    case "FileName":
                        descriptor.PluginFileName = value;
                        break;
                    case "LimitedToStores":
                        {
                            foreach (var str in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimEnd()))
                            {
                                int storeId;
                                int.TryParse(str, out storeId);
                                descriptor.LimitedToStores.Add(storeId);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (descriptor.SupportedVersions.Count == 0)
            {
                descriptor.SupportedVersions.Add("2.00");
            }
            return descriptor;
        }

        /// <summary>
        /// 保存插件描述到文件中
        /// </summary>
        /// <param name="plugin"></param>
        public static void SavePluginDescriptionFile(PluginDescriptor plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentException("plugin");
            }

            if (plugin.OriginalAssemblyFile == null)
            {
                throw new NopException(string.Format("不能加载原始的程序集文件路径：{0}", plugin.SystemName));
            }

            var path = Path.Combine(plugin.OriginalAssemblyFile.Directory.FullName, "Descriptor.txt");
            if (!File.Exists(path))
                throw new NopException(string.Format("插件{0}的描述文件{1}不存在", plugin.SystemName, path));

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("Group", plugin.Group));
            keyValues.Add(new KeyValuePair<string, string>("FriendlyName", plugin.FriendlyName));
            keyValues.Add(new KeyValuePair<string, string>("SystemName", plugin.SystemName));
            keyValues.Add(new KeyValuePair<string, string>("Version", plugin.Version));
            keyValues.Add(new KeyValuePair<string, string>("SurpportedVersions", string.Join(",", plugin.SupportedVersions)));
            keyValues.Add(new KeyValuePair<string, string>("Author", plugin.Author));
            keyValues.Add(new KeyValuePair<string, string>("DisplayOrder", plugin.DisplayOrder.ToString()));
            keyValues.Add(new KeyValuePair<string, string>("FileName", plugin.PluginFileName));
            if (plugin.LimitedToStores.Count > 0)
            {
                var storeList = string.Join(",", plugin.LimitedToStores);
                keyValues.Add(new KeyValuePair<string, string>("LimitedToStores", storeList));
            }

            var sb = new StringBuilder();
            for (int i = 0; i < keyValues.Count; i++)
            {
                var key = keyValues[i].Key;
                var value = keyValues[i].Value;
                sb.AppendFormat("{0}: {1}", key, value);
                if (i != keyValues.Count - 1)
                {
                    sb.Append(Environment.NewLine);
                }
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}
