using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure
{
    /// <summary>
    /// 查找对饮的类型
    /// </summary>
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies();
    }
}
