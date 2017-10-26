using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Localization;
using Core.Data;
using Core.Caching;
using System.Reflection;

namespace Services.Localization
{
    public class LocalizedEntityService : ILocalizedEntityService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        private const string LOCALIZEDPROPERTY_KEY = "Nop.localizedproperty.value-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for caching
        /// </summary>
        private const string LOCALIZEDPROPERTY_ALL_KEY = "Nop.localizedproperty.all";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string LOCALIZEDPROPERTY_PATTERN_KEY = "Nop.localizedproperty.";

        #endregion

        #region Fields

        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly ICacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="localizationSettings">Localization settings</param>
        public LocalizedEntityService(ICacheManager cacheManager,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            LocalizationSettings localizationSettings)
        {
            this._cacheManager = cacheManager;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class LocalizedPropertyForCaching
        {
            public int Id { get; set; }
            public int EntityId { get; set; }
            public int LanguageId { get; set; }
            public string LocaleKeyGroup { get; set; }
            public string LocaleKey { get; set; }
            public string LocaleValue { get; set; }
        }

        #endregion

        #region Utilities

        protected virtual IList<LocalizedProperty> GetLocalizedProperties(int entityId, string localeKeyGroup)
        {
            if (entityId == 0 || string.IsNullOrEmpty(localeKeyGroup))
                return new List<LocalizedProperty>();

            var query = from lp in _localizedPropertyRepository.Table
                        where lp.EntityId == entityId && lp.LocaleKeyGroup == localeKeyGroup
                        select lp;
            return query.ToList();
        }

        protected virtual IList<LocalizedPropertyForCaching> GetAllLocalizedPropertiesCached()
        {
            string key = string.Format(LOCALIZEDPROPERTY_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from lp in _localizedPropertyRepository.Table
                            select lp;
                var localizedProperties = query.ToList();
                var list = new List<LocalizedPropertyForCaching>();
                foreach (var lp in localizedProperties)
                {
                    var localizedPropertyCaching = new LocalizedPropertyForCaching()
                    {
                        Id = lp.Id,
                        EntityId = lp.EntityId,
                        LanguageId = lp.LanguageId,
                        LocaleKeyGroup = lp.LocaleKeyGroup,
                        LocaleKey = lp.LocaleKey,
                        LocaleValue = lp.LocaleValue
                    };
                    list.Add(localizedPropertyCaching);
                }
                return list;
            });
        }

        #endregion

        public void DeleteLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentException("localizedProperty");

            _localizedPropertyRepository.Delete(localizedProperty);

            _cacheManager.RemoveByPattern(LOCALIZEDPROPERTY_PATTERN_KEY);
        }

        public LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId)
        {
            if (localizedPropertyId == 0)
                return null;

            return _localizedPropertyRepository.GetById(localizedPropertyId);
        }

        public string GetLocalizedValue(int languageId, int entityId, string localeKeyGroup, string localeKey)
        {
            if (_localizationSettings.LoadAllLocalizedPropertiesOnStartup)
            {
                string key = string.Format(LOCALIZEDPROPERTY_KEY, languageId, entityId, localeKeyGroup, localeKey);
                return _cacheManager.Get(key, () =>
                {
                    var source = GetAllLocalizedPropertiesCached();
                    var query = from lp in source
                                where lp.LanguageId == languageId && lp.EntityId == entityId && lp.LocaleKeyGroup == localeKeyGroup && lp.LocaleKey == localeKey
                                select lp;
                    var localeValue = query.FirstOrDefault();
                    if (localeValue == null)
                        return "";
                    return localeValue.LocaleValue;
                });
            }
            else
            {
                string key = string.Format(LOCALIZEDPROPERTY_KEY, languageId, entityId, localeKeyGroup, localeKey);
                return _cacheManager.Get(key, () =>
                {
                    var query = from lp in _localizedPropertyRepository.Table
                                where lp.LanguageId == languageId && lp.EntityId == entityId && lp.LocaleKeyGroup == localeKeyGroup && lp.LocaleKey == localeKey
                                select lp;
                    var localeValue = query.FirstOrDefault();
                    if (localeValue == null)
                        return "";
                    return localeValue.LocaleValue;
                });
            }
        }

        public void InsertLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentException("localizedProperty");

            _localizedPropertyRepository.Insert(localizedProperty);

            _cacheManager.RemoveByPattern(LOCALIZEDPROPERTY_PATTERN_KEY);
        }

        public void SaveLocalizedValue<T>(T entity, Expression<Func<T, string>> keySelector, string localeValue, int languageId) where T : BaseEntity, ILocalizedEntity
        {
            SaveLocalizedValue<T, string>(entity, keySelector, localeValue, languageId);
        }

        public void SaveLocalizedValue<T, TPropType>(T entity, Expression<Func<T, TPropType>> keySelector, TPropType localeValue, int languageId) where T : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (languageId == 0)
                throw new ArgumentOutOfRangeException("languageId 不能为0");

            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("表达式{0}应为属性或字段的访问器", keySelector));

            var proInfo = member.Member as PropertyInfo;
            if (proInfo == null)
                throw new ArgumentException(string.Format("表达式{0}应为属性或字段的访问器", keySelector));

            string localeKeyGroup = typeof(T).Name;
            string localeKey = proInfo.Name;
            var props = GetLocalizedProperties(entity.Id, localeKeyGroup);
            var prop = props.FirstOrDefault(m => m.LanguageId == languageId && m.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase));

            var localeValueStr = CommonHelper.To<string>(localeValue);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(localeValueStr))
                {
                    DeleteLocalizedProperty(prop);
                }
                else
                {
                    prop.LocaleValue = localeValueStr;
                    UpdateLocalizedProperty(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(localeValueStr))
                {
                    prop = new LocalizedProperty()
                    {
                        EntityId = entity.Id,
                        LanguageId = languageId,
                        LocaleKey = localeKey,
                        LocaleKeyGroup = localeKeyGroup,
                        LocaleValue = localeValueStr
                    };
                    InsertLocalizedProperty(prop);
                }
            }
        }

        public void UpdateLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentException("localizedProperty");

            _localizedPropertyRepository.Update(localizedProperty);

            _cacheManager.RemoveByPattern(LOCALIZEDPROPERTY_PATTERN_KEY);
        }
    }
}
