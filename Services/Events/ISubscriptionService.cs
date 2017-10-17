using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Events
{
    /// <summary>
    /// 事件订阅服务接口
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// 获取所有的事件订阅者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<IConsumer<T>> GetSubscriptions<T>();
    }
}
