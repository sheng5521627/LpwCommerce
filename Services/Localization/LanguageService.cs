using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Localization;
using Core.Caching;
using Core.Data;
using Services.Stores;
using Services.Configuration;
using Services.Events;

namespace Services.Localization
{
    public partial class LanguageService : ILanguageService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        private const string LANGUAGES_BY_ID_KEY = "Nop.language.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string LANGUAGES_ALL_KEY = "Nop.language.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string LANGUAGES_PATTERN_KEY = "Nop.language.";

        #endregion

        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository,
            IStoreMappingService storeMappingService,
            ISettingService settingService,
            LocalizationSettings localizationSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._languageRepository = languageRepository;
            this._storeMappingService = storeMappingService;
            this._settingService = settingService;
            this._localizationSettings = localizationSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void DeleteLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentException("language");

            if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            {
                foreach (var activeLanguage in GetAllLanguages())
                {
                    if (activeLanguage.Id != language.Id)
                    {
                        _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                        _settingService.SaveSetting(_localizationSettings);
                        break;
                    }
                }
            }

            _languageRepository.Delete(language);

            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            _eventPublisher.EntityDeleted(language);
        }

        public IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            var languages = _cacheManager.Get(key, () =>
            {
                var query = _languageRepository.Table;
                if (!showHidden)
                    query = query.Where(m => m.Published);

                query = query.OrderBy(m => m.DisplayOrder);
                return query.ToList();
            });

            if (storeId > 0)
            {
                languages = languages.Where(m => _storeMappingService.Authorize(m, storeId)).ToList();
            }

            return languages;
        }

        public Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, () =>
            {
                return _languageRepository.GetById(languageId);
            });
        }

        public void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentException("language");

            _languageRepository.Delete(language);

            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            _eventPublisher.EntityInserted(language);
        }

        public void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentException("language");

            _languageRepository.Update(language);

            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            _eventPublisher.EntityUpdated(language);
        }
    }
}
