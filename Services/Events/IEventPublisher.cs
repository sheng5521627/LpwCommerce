using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Events
{
    /// <summary>
    /// 时间发布者
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventMessage"></param>
        void Publish<T>(T eventMessage);
    }
}
