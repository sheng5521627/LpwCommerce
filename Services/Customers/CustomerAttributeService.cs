using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Customers;
using Core.Data;
using Services.Events;
using Core.Caching;

namespace Services.Customers
{
    public partial class CustomerAttributeService : ICustomerAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string CUSTOMERATTRIBUTES_ALL_KEY = "Nop.customerattribute.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        private const string CUSTOMERATTRIBUTES_BY_ID_KEY = "Nop.customerattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        private const string CUSTOMERATTRIBUTEVALUES_ALL_KEY = "Nop.customerattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute value ID
        /// </remarks>
        private const string CUSTOMERATTRIBUTEVALUES_BY_ID_KEY = "Nop.customerattributevalue.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERATTRIBUTES_PATTERN_KEY = "Nop.customerattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERATTRIBUTEVALUES_PATTERN_KEY = "Nop.customerattributevalue.";
        #endregion

        #region Fields

        private readonly IRepository<CustomerAttribute> _customerAttributeRepository;
        private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="customerAttributeRepository">Customer attribute repository</param>
        /// <param name="customerAttributeValueRepository">Customer attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CustomerAttributeService(ICacheManager cacheManager,
            IRepository<CustomerAttribute> customerAttributeRepository,
            IRepository<CustomerAttributeValue> customerAttributeValueRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._customerAttributeRepository = customerAttributeRepository;
            this._customerAttributeValueRepository = customerAttributeValueRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void DeleteCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttibute");

            _customerAttributeRepository.Delete(customerAttribute);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);

            _eventPublisher.EntityDeleted(customerAttribute);
        }

        public void DeleteCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException("customerAttributeValue");

            _customerAttributeValueRepository.Delete(customerAttributeValue);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

            _eventPublisher.EntityDeleted(customerAttributeValue);
        }

        public IList<CustomerAttribute> GetAllCustomerAttributes()
        {
            string key = string.Format(CUSTOMERATTRIBUTES_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _customerAttributeRepository.Table
                            orderby c.DisplayOrder
                            select c;
                return query.ToList();
            });
        }

        public CustomerAttribute GetCustomerAttributeById(int customerAttributeId)
        {
            if (customerAttributeId == 0)
                return null;

            string key = string.Format(CUSTOMERATTRIBUTES_BY_ID_KEY, customerAttributeId);
            return _cacheManager.Get(key, () => { return _customerAttributeRepository.GetById(customerAttributeId); });
        }

        public CustomerAttributeValue GetCustomerAttributeValueById(int customerAttributeValueId)
        {
            if (customerAttributeValueId == 0)
                return null;

            string key = string.Format(CUSTOMERATTRIBUTEVALUES_BY_ID_KEY, customerAttributeValueId);
            return _cacheManager.Get(key, () => { return _customerAttributeValueRepository.GetById(customerAttributeValueId); });
        }

        public IList<CustomerAttributeValue> GetCustomerAttributeValues(int customerAttributeId)
        {
            if (customerAttributeId == 0)
                return new List<CustomerAttributeValue>();

            string key = string.Format(CUSTOMERATTRIBUTEVALUES_ALL_KEY, customerAttributeId);
            return _cacheManager.Get(key, () => 
            {
                var query = from crv in _customerAttributeValueRepository.Table
                            orderby crv.DisplayOrder
                            where crv.CustomerAttributeId == customerAttributeId
                            select crv;
                return query.ToList();
            });
        }

        public void InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Delete(customerAttribute);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);

            _eventPublisher.EntityInserted(customerAttribute);
        }

        public void InsertCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException("customerAttributeValue");

            _customerAttributeValueRepository.Insert(customerAttributeValue);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

            _eventPublisher.EntityInserted(customerAttributeValue);
        }

        public void UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Update(customerAttribute);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);

            _eventPublisher.EntityUpdated(customerAttribute);
        }

        public void UpdateCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException("customerAttributeValue");

            _customerAttributeValueRepository.Update(customerAttributeValue);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);

            _eventPublisher.EntityUpdated(customerAttributeValue);
        }
    }
}
