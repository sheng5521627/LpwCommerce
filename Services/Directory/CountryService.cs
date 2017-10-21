using System;
using System.Collections.Generic;
using Core.Domain.Directory;
using Core;
using Core.Data;
using Services.Events;
using Core.Caching;
using Core.Domain.Catalog;
using Core.Domain.Stores;
using System.Linq;
using Services.Localization;

namespace Services.Directory
{
    public partial class CountryService : ICountryService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : show hidden records?
        /// </remarks>
        private const string COUNTRIES_ALL_KEY = "Nop.country.all-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string COUNTRIES_PATTERN_KEY = "Nop.country.";

        #endregion

        #region Fields

        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreContext _storeContext;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="countryRepository">Country repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public CountryService(ICacheManager cacheManager,
            IRepository<Country> countryRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStoreContext storeContext,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._countryRepository = countryRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._storeContext = storeContext;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void DeleteCountry(Country country)
        {
            if (country == null)
                throw new ArgumentException("country");

            _countryRepository.Delete(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            _eventPublisher.EntityDeleted(country);
        }

        public IList<Country> GetAllCountries(int languageId = 0, bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_ALL_KEY, languageId, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _countryRepository.Table;
                if (!showHidden)
                    query = query.Where(m => m.Published);
                query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Name);

                if (!showHidden && !_catalogSettings.IgnoreStoreLimitations)
                {
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from c in query
                            join sc in _storeMappingRepository.Table
                            on new { c1 = c.Id, c2 = "Country" } equals new { c1 = sc.EntityId, c2 = sc.EntityName } into c_sc
                            from sc in c_sc.DefaultIfEmpty()
                            where !c.LimitedToStores || sc.StoreId == currentStoreId
                            select c;
                    query = from c in query
                            group c by c.Id into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Name);
                }

                var contries = query.ToList();
                if (languageId > 0)
                {
                    contries = contries.OrderBy(m => m.DisplayOrder).ThenBy(m => m.GetLocalized(x => x.Name, languageId)).ToList();
                }
                return contries;
            });
        }

        public IList<Country> GetAllCountriesForBilling(int languageId = 0, bool showHidden = false)
        {
            return GetAllCountries().Where(m => m.AllowsBilling).ToList();
        }

        public IList<Country> GetAllCountriesForShipping(int languageId = 0, bool showHidden = false)
        {
            return GetAllCountries().Where(m => m.AllowsShipping).ToList();
        }

        public IList<Country> GetCountriesByIds(int[] countryIds)
        {
            if (countryIds == null || countryIds.Length == 0)
                return new List<Country>();

            var query = from c in _countryRepository.Table
                        where countryIds.Contains(c.Id)
                        select c;
            var countries = query.ToList();
            var sortedCountries = new List<Country>();
            foreach (var id in countryIds)
            {
                var country = countries.Find(m => m.Id == id);
                if (country != null)
                    sortedCountries.Add(country);
            }

            return sortedCountries;
        }

        public Country GetCountryById(int countryId)
        {
            if (countryId == 0)
                return null;

            return _countryRepository.GetById(countryId);
        }

        public Country GetCountryByThreedLetterIsoCode(string threedLetterIsoCode)
        {
            if (String.IsNullOrEmpty(threedLetterIsoCode))
                return null;

            var query = from country in _countryRepository.Table
                        where country.ThreeLetterIsoCode == threedLetterIsoCode
                        select country;
            return query.FirstOrDefault();
        }

        public Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode)
        {
            if (string.IsNullOrEmpty(twoLetterIsoCode))
                return null;

            var query = from country in _countryRepository.Table
                        where country.TwoLetterIsoCode == twoLetterIsoCode
                        select country;

            return query.FirstOrDefault();
        }

        public void InsertCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Insert(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            _eventPublisher.EntityInserted(country);
        }

        public void UpdateCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Update(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            _eventPublisher.EntityUpdated(country);
        }
    }
}