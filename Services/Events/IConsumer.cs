using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Events
{
    /// <summary>
    /// 消费者接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsumer<T>
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventMessage"></param>
        void HandleEvent(T eventMessage);
    }
}
