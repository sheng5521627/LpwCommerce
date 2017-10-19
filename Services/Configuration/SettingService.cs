using Core;
using Core.Caching;
using Core.Configuration;
using Core.Data;
using Core.Domain.Configuration;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _settingRepository.TableNoTracking
                            orderby s.Name, s.StoreId
                            select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
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

        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentException("setting");

            _settingRepository.Insert(setting);
            if (clearCache)
            {
                _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);
            }

            _eventPublisher.EntityUpdated(setting);
        }

        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentException("setting");

            _settingRepository.Update(setting);

            if (clearCache)
            {
                _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);
            }

            _eventPublisher.EntityUpdated(setting);
        }

        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentException("setting");

            _settingRepository.Delete(setting);
            _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

            _eventPublisher.EntityDeleted(setting);
        }

        public virtual Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            return _settingRepository.GetById(settingId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="storeId"></param>
        /// <param name="loadShareValueIfNotFound"></param>
        /// <returns></returns>
        public virtual Setting GetSetting(string key, int storeId = 0, bool loadShareValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);
                if (setting == null && storeId > 0 && loadShareValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);
                if (setting != null)
                    return GetSettingById(setting.Id);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="storeId"></param>
        /// <param name="loadShareValueIfNotFound"></param>
        /// <returns></returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), int storeId = 0, bool loadShareValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);
                if (setting == null && storeId > 0 && loadShareValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);
                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }
            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="storeId"></param>
        /// <param name="clreaCache"></param>
        public virtual void SetSetting<T>(string key, T value, int storeId = 0, bool clreaCache = true)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");
            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetNopCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ? allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching != null)
            {
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSetting(setting);
            }
            else
            {
                var setting = new Setting()
                {
                    Name = key,
                    Value = valueStr,
                    StoreId = storeId
                };
                InsertSetting(setting);
            }
        }

        public virtual IList<Setting> GetAllSettings()
        {
            var query = from s in _settingRepository.Table
                        orderby s.Name, s.StoreId
                        select s;
            return query.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="settings"></param>
        /// <param name="keySelector"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public virtual bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0)
            where T : ISettings, new()
        {
            var key = settings.GetSettingKey(keySelector);
            var setting = GetSettingByKey<string>(key, storeId: storeId);
            return setting != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public virtual T LoadSetting<T>(int storeId = 0) where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanWrite || !prop.CanRead)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var setting = GetSettingByKey<string>(key, storeId: storeId, loadShareValueIfNotFound: true);
                if (setting == null)
                    continue;
                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;
                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;
                object value = CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                prop.SetValue(settings, value, null);
            }
            return settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        /// <param name="storeId"></param>
        public virtual void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;
                if (!CommonHelper.GetNopCustomTypeConverter(typeof(T)).CanConvertFrom(typeof(string)))
                    continue;
                string key = typeof(T).Name + "." + prop.Name;
                dynamic value = prop.GetValue(settings, null);
                if (value != "")
                    SetSetting(key, value, storeId, false);
                else
                    SetSetting(key, "", storeId, false);
            }
            ClearCache();
        }

        public virtual void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = settings.GetSettingKey(keySelector);
            dynamic value = propInfo.GetValue(settings);
            if (value != null)
                SetSetting(key, value, storeId, clearCache);
            else
                SetSetting(key, "", storeId, clearCache);
        }

        // <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSetting<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = GetAllSettings();
            foreach (var prop in typeof(T).GetProperties())
            {
                string key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            foreach (var setting in settingsToDelete)
                DeleteSetting(setting);
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        public virtual void DeleteSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new()
        {
            string key = settings.GetSettingKey(keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                DeleteSetting(setting);
            }
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);
        }
    }
}
