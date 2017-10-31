using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Web.Framework.Themes
{
    public class ThemeConfiguration
    {
        public string ThemeTitle { get; set; }

        public string ThemeName { get; set; }

        public bool SupportRtl { get; set; }

        public string PreviewText { get; set; }

        public string PreviewImageUrl { get; set; }

        public string Path { get; set; }

        public XmlNode ConfigurationNode { get; set; }

        public ThemeConfiguration(string themeName,string path,XmlDocument doc)
        {
            ThemeName = themeName;
            Path = path;
            var node = doc.SelectSingleNode("Theme");

            if (node != null)
            {
                ConfigurationNode = node;
                var attribute = node.Attributes["title"];
                ThemeTitle = attribute == null ? string.Empty : attribute.Value;
                attribute = node.Attributes["supportRTL"];
                SupportRtl = attribute == null ? false : bool.Parse(attribute.Value);
                attribute = node.Attributes["previewImageUrl"];
                PreviewImageUrl = attribute == null ? string.Empty : attribute.Value;
                attribute = node.Attributes["previewText"];
                PreviewText = attribute == null ? string.Empty : attribute.Value;
            }
        }
    }
}
