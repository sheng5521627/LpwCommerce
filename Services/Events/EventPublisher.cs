using Core.Infrastructure;
using Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Logging;

namespace Services.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;
        public EventPublisher(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// 发布给消费者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="eventMessage"></param>
        protected virtual void PublishToConsumer<T>(IConsumer<T> x,T eventMessage)
        {
            var plugin = FingPlugin(x.GetType());
            if (plugin != null && !plugin.Installed)
                return;
            try
            {
                x.HandleEvent(eventMessage);
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                try
                {
                    logger.Error(ex.Message, ex);
                }
                catch (Exception)
                {
                    
                }
            }
        }

        /// <summary>
        /// 查找插件
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        protected virtual PluginDescriptor FingPlugin(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentException("providerType");

            if (PluginManager.ReferencedPlugins == null)
                return null;

            foreach(var plugin in PluginManager.ReferencedPlugins)
            {
                if (plugin.RefrencedAssembly == null)
                    return null;

                if (plugin.RefrencedAssembly.FullName == providerType.Assembly.FullName)
                    return plugin;
            }
            return null;
        }

        public void Publish<T>(T eventMessage)
        {
            var subscriptions = _subscriptionService.GetSubscriptions<T>();
            subscriptions.ToList().ForEach(x => 
            {
                PublishToConsumer(x, eventMessage);
            });
        }
    }
}
