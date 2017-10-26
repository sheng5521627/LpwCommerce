﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Orders;
using Core.Data;
using Services.Stores;
using Services.Events;
using Core.Caching;

namespace Services.Orders
{
    /// <summary>
    /// Checkout attribute service
    /// </summary>
    public partial class CheckoutAttributeService : ICheckoutAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : >A value indicating whether we should exlude shippable attributes
        /// </remarks>
        private const string CHECKOUTATTRIBUTES_ALL_KEY = "Nop.checkoutattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        private const string CHECKOUTATTRIBUTES_BY_ID_KEY = "Nop.checkoutattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        private const string CHECKOUTATTRIBUTEVALUES_ALL_KEY = "Nop.checkoutattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute value ID
        /// </remarks>
        private const string CHECKOUTATTRIBUTEVALUES_BY_ID_KEY = "Nop.checkoutattributevalue.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CHECKOUTATTRIBUTES_PATTERN_KEY = "Nop.checkoutattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CHECKOUTATTRIBUTEVALUES_PATTERN_KEY = "Nop.checkoutattributevalue.";
        #endregion

        #region Fields

        private readonly IRepository<CheckoutAttribute> _checkoutAttributeRepository;
        private readonly IRepository<CheckoutAttributeValue> _checkoutAttributeValueRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="checkoutAttributeRepository">Checkout attribute repository</param>
        /// <param name="checkoutAttributeValueRepository">Checkout attribute value repository</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="eventPublisher">Event published</param>
        public CheckoutAttributeService(ICacheManager cacheManager,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<CheckoutAttributeValue> checkoutAttributeValueRepository,
            IStoreMappingService storeMappingService,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._checkoutAttributeRepository = checkoutAttributeRepository;
            this._checkoutAttributeValueRepository = checkoutAttributeValueRepository;
            this._storeMappingService = storeMappingService;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void DeleteCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            _checkoutAttributeRepository.Delete(checkoutAttribute);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttribute);
        }

        public IList<CheckoutAttribute> GetAllCheckoutAttributes(int storeId = 0, bool excludeShippableAttributes = false)
        {
            string key = string.Format(CHECKOUTATTRIBUTES_ALL_KEY, storeId, excludeShippableAttributes);
            return _cacheManager.Get(key, () => 
            {
                var query = from ca in _checkoutAttributeRepository.Table
                            orderby ca.DisplayOrder
                            select ca;
                var checkoutAttributes = query.ToList();
                if (storeId > 0)
                {
                    checkoutAttributes = checkoutAttributes.Where(m => _storeMappingService.Authorize(m)).ToList();
                }
                if (excludeShippableAttributes)
                {
                    checkoutAttributes = checkoutAttributes.RemoveShippableAttributes().ToList();
                }
                return checkoutAttributes;
            });
        }

        public CheckoutAttribute GetCheckoutAttributeById(int checkoutAttributeId)
        {
            if (checkoutAttributeId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTES_BY_ID_KEY, checkoutAttributeId);
            return _cacheManager.Get(key, () => _checkoutAttributeRepository.GetById(checkoutAttributeId));
        }

        public void InsertCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            _checkoutAttributeRepository.Insert(checkoutAttribute);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttribute);
        }

        public void UpdateCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            _checkoutAttributeRepository.Update(checkoutAttribute);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttribute);
        }

        public void DeleteCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            _checkoutAttributeValueRepository.Delete(checkoutAttributeValue);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttributeValue);
        }

        public IList<CheckoutAttributeValue> GetCheckoutAttributeValues(int checkoutAttributeId)
        {
            string key = string.Format(CHECKOUTATTRIBUTEVALUES_ALL_KEY, checkoutAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _checkoutAttributeValueRepository.Table
                            orderby cav.DisplayOrder
                            where cav.CheckoutAttributeId == checkoutAttributeId
                            select cav;
                var checkoutAttributeValues = query.ToList();
                return checkoutAttributeValues;
            });
        }

        public CheckoutAttributeValue GetCheckoutAttributeValueById(int checkoutAttributeValueId)
        {
            if (checkoutAttributeValueId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTEVALUES_BY_ID_KEY, checkoutAttributeValueId);
            return _cacheManager.Get(key, () => _checkoutAttributeValueRepository.GetById(checkoutAttributeValueId));
        }

        public void InsertCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            _checkoutAttributeValueRepository.Insert(checkoutAttributeValue);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttributeValue);
        }

        public void UpdateCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            _checkoutAttributeValueRepository.Update(checkoutAttributeValue);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttributeValue);
        }        
    }
}
