﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Tax;
using Core.Data;
using Services.Events;
using Core.Caching;

namespace Services.Tax
{
    /// <summary>
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string TAXCATEGORIES_ALL_KEY = "Nop.taxcategory.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tax category ID
        /// </remarks>
        private const string TAXCATEGORIES_BY_ID_KEY = "Nop.taxcategory.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string TAXCATEGORIES_PATTERN_KEY = "Nop.taxcategory.";

        #endregion

        #region Fields

        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="taxCategoryRepository">Tax category repository</param>
        /// <param name="eventPublisher">Event published</param>
        public TaxCategoryService(ICacheManager cacheManager,
            IRepository<TaxCategory> taxCategoryRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _taxCategoryRepository = taxCategoryRepository;
            _eventPublisher = eventPublisher;
        }

        public void DeleteTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException("taxCategory");

            _taxCategoryRepository.Delete(taxCategory);

            _cacheManager.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(taxCategory);
        }

        public IList<TaxCategory> GetAllTaxCategories()
        {
            string key = string.Format(TAXCATEGORIES_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from tc in _taxCategoryRepository.Table
                            orderby tc.DisplayOrder
                            select tc;
                var taxCategories = query.ToList();
                return taxCategories;
            });
        }

        public TaxCategory GetTaxCategoryById(int taxCategoryId)
        {
            if (taxCategoryId == 0)
                return null;

            string key = string.Format(TAXCATEGORIES_BY_ID_KEY, taxCategoryId);
            return _cacheManager.Get(key, () => _taxCategoryRepository.GetById(taxCategoryId));
        }

        public void InsertTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException("taxCategory");

            _taxCategoryRepository.Insert(taxCategory);

            _cacheManager.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(taxCategory);
        }

        public void UpdateTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException("taxCategory");

            _taxCategoryRepository.Update(taxCategory);

            _cacheManager.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(taxCategory);
        }

        #endregion
    }
}
