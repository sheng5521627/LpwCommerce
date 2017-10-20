using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Localization;
using Core.Data;
using Core;
using Services.Logging;
using Core.Caching;
using Data;
using Core.Domain.Common;
using Services.Events;

namespace Services.Localization
{
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.lsr.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "Nop.lsr.{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Nop.lsr.";

        #endregion

        #region Fields

        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ILanguageService _languageService;
        private readonly ICacheManager _cacheManager;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="logger">Logger</param>
        /// <param name="workContext">Work context</param>
        /// <param name="lsrRepository">Locale string resource repository</param>
        /// <param name="languageService">Language service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LocalizationService(ICacheManager cacheManager,
            ILogger logger, IWorkContext workContext,
            IRepository<LocaleStringResource> lsrRepository,
            ILanguageService languageService,
            IDataProvider dataProvider, IDbContext dbContext, CommonSettings commonSettings,
            LocalizationSettings localizationSettings, IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._logger = logger;
            this._workContext = workContext;
            this._lsrRepository = lsrRepository;
            this._languageService = languageService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._localizationSettings = localizationSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void DeleteLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRepository.Delete(localeStringResource);

            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            _eventPublisher.EntityDeleted(localeStringResource);
        }

        public string ExportResourceToXml(Language language)
        {
            throw new NotImplementedException();
        }

        public IList<LocaleStringResource> GetAllResource(int languageId)
        {
            var query = from lsr in _lsrRepository.Table
                        orderby lsr.ResourceName
                        where lsr.LanguageId == languageId
                        select lsr;

            return query.ToList();
        }

        public Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            return _cacheManager.Get(key, () => 
            {
                var query = from lsr in _lsrRepository.Table
                            orderby lsr.ResourceName
                            where lsr.LanguageId == languageId
                            select lsr;
                var locales = query.ToList();
                var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
                foreach(var locale in locales)
                {
                    var resourceName = locale.ResourceName.ToLowerInvariant();
                    if (!dictionary.ContainsKey(resourceName))
                        dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceValue));
                }
                return dictionary;
            });
        }

        public LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            return _lsrRepository.GetById(localeStringResourceId);
        }

        public LocaleStringResource GetLocaleStringResourceByName(string resourceName)
        {
            if (_workContext.WorkingLanguage != null)
                return GetLocaleStringResourceByName(resourceName, _workContext.WorkingLanguage.Id);
            return null;
        }

        public LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId, bool logIfNotFound = true)
        {
            var query = from lsr in _lsrRepository.Table
                        where lsr.ResourceName == resourceName && lsr.LanguageId == languageId
                        orderby lsr.ResourceName
                        select lsr;
            var localeStringResource = query.FirstOrDefault();

            if(localeStringResource ==null && logIfNotFound)
            {
                _logger.Warning(string.Format("Resource string ({0}) not found. Language ID = {1}", resourceName, languageId));
            }
            return localeStringResource;
        }

        public string GetResource(string resourceKey)
        {
            if (_workContext.WorkingLanguage != null)
                return GetResource(resourceKey, _workContext.WorkingLanguage.Id);
            return null;
        }

        public string GetResource(string resourceKey, int languageId, bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            string result = string.Empty;
            if (resourceKey == null)
                throw new ArgumentException("resourceKey");
            resourceKey = resourceKey.Trim().ToLowerInvariant();

            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                var resoures = GetAllResourceValues(languageId);
                if (resoures.ContainsKey(resourceKey))
                {
                    result = resoures[resourceKey].Value;
                }
            }
            else
            {
                string key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, languageId, resourceKey);
                string lsr = _cacheManager.Get(key, () => 
                {
                    var query = from l in _lsrRepository.Table
                                where l.LanguageId == languageId && l.ResourceName == resourceKey
                                select l.ResourceValue;
                    return query.FirstOrDefault();
                });
                if (lsr != null)
                    return lsr;
            }
            if (string.IsNullOrEmpty(result))
            {
                if(logIfNotFound)
                    _logger.Warning(string.Format("Resource string ({0}) is not found. Language ID = {1}", resourceKey, languageId));
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    return defaultValue;
                }
                else
                {
                    if (!returnEmptyIfNotFound)
                        result = resourceKey;
                }
            }
            return result;
        }

        public void ImportResourceFromXml(Language language, string xml)
        {
            throw new NotImplementedException();
        }

        public void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRepository.Insert(localeStringResource);

            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            _eventPublisher.EntityInserted(localeStringResource);
        }

        public void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRepository.Update(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(localeStringResource);
        }
    }
}
