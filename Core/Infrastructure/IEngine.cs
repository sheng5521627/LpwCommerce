using Core.Configuration;
using Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure
{
    /// <summary>
    /// 整个应用程序的启动入口
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// 依赖注入容器管理
        /// </summary>
        ContainerManager ContainnerManager { get; }

        /// <summary>
        /// 组件和插件初始化
        /// </summary>
        /// <param name="config"></param>
        void Initialize(NopConfig config);

        /// <summary>
        /// 解析注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
    }
}
