using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Framework.Themes
{
    public interface IThemeContext
    {
        /// <summary>
        /// 当前主题系统名称
        /// </summary>
        string WorkingThemeName { get; set; }
    }
}
