using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Core.Data
{
    public partial class DataSettingsManager
    {
        protected const string separator = ":";

        protected const string fileName = "Settings.txt";

        protected virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
                return HostingEnvironment.MapPath(path);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected virtual DataSettings ParseSettings(string text)
        {
            var shellSettings = new DataSettings();
            if (string.IsNullOrEmpty(text))
                return shellSettings;

            var settings = new List<string>();
            using(var reader = new StringReader(text))
            {
                string str;
                while((str = reader.ReadLine()) != null)
                {
                    settings.Add(str);
                }
            }
            foreach(var setting in settings)
            {
                var separatorIndex = setting.IndexOf(separator);
                if (separatorIndex == -1)
                    continue;

                string key = setting.Substring(0, separatorIndex);
                string value = setting.Substring(separatorIndex + 1);

                switch (key)
                {
                    case "DataProvider":
                        shellSettings.DataProvider = value;
                        break;
                    case "DataConnectionString":
                        shellSettings.DataConnectionString = value;
                        break;
                    default:
                        shellSettings.RawDataSettings.Add(key, value);
                        break;
                }
            }
            return shellSettings;
        }

        /// <summary>
        /// 组合
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected virtual string ComposeSettings(DataSettings settings)
        {
            if (settings == null)
                return "";
            return string.Format("DataProvider:{0}{2}DataConnectionString:{1}{2}", settings.DataProvider, settings.DataConnectionString, Environment.NewLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public virtual DataSettings LoadSettings(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(MapPath("~/App_Data/"), fileName);
            }
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                return ParseSettings(text);
            }
            return new DataSettings();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public virtual void SaveSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentException("settings");

            string filePath = Path.Combine(MapPath("~/App_Data/"), fileName);
            if(!File.Exists(filePath))
            {
                using (File.Create(filePath))
                {

                }
            }
            var text = ComposeSettings(settings);
            File.WriteAllText(filePath, text);
        }
    }
}
