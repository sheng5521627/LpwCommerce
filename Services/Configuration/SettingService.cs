using Core.Caching;
using Core.Data;
using Core.Domain.Configuration;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Configuration
{
    public partial class SettingService : ISettingService
    {
        #region Const

        private const string SETTINGS_ALL_KEY = "Nop.setting.all";
        private const string SETTINGS_PATTERN_KEY = "Nop.setting";

        #endregion

        #region Fields

        private readonly IRepository<Setting> _settingRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        public SettingService(ICacheManager cacheManager, IEventPublisher eventPublisher,
            IRepository<Setting> settingRepository)
        {
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._settingRepository = settingRepository;
        }

        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int StoreId { get; set; }
        }

        /// <summary>
        /// 从缓存中获取配置信息
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string,IList<SettingForCaching>> GetAllSettingsCached()
        {
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _settingRepository.TableNoTracking
                            orderby s.Name, s.StoreId
                            select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach(var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        StoreId = s.StoreId
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        dictionary.Add(resourceName, new List<SettingForCaching>() { settingForCaching });
                    }
                    else
                    {
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });             
        }

        public virtual void InsertSetting(Setting setting,bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentException("setting");
        }
    }
}
