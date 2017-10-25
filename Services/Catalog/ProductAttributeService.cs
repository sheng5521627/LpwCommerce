using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Catalog;
using Core.Data;
using Services.Events;
using Core.Caching;

namespace Services.Catalog
{
    /// <summary>
    /// Product attribute service
    /// </summary>
    public partial class ProductAttributeService : IProductAttributeService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        private const string PRODUCTATTRIBUTES_ALL_KEY = "Nop.productattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute ID
        /// </remarks>
        private const string PRODUCTATTRIBUTES_BY_ID_KEY = "Nop.productattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTATTRIBUTEMAPPINGS_ALL_KEY = "Nop.productattributemapping.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        private const string PRODUCTATTRIBUTEMAPPINGS_BY_ID_KEY = "Nop.productattributemapping.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        private const string PRODUCTATTRIBUTEVALUES_ALL_KEY = "Nop.productattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute value ID
        /// </remarks>
        private const string PRODUCTATTRIBUTEVALUES_BY_ID_KEY = "Nop.productattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTATTRIBUTECOMBINATIONS_ALL_KEY = "Nop.productattributecombination.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTATTRIBUTES_PATTERN_KEY = "Nop.productattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY = "Nop.productattributemapping.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTATTRIBUTEVALUES_PATTERN_KEY = "Nop.productattributevalue.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY = "Nop.productattributecombination.";

        #endregion

        #region Fields

        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly IRepository<PredefinedProductAttributeValue> _predefinedProductAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;


        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productAttributeRepository">Product attribute repository</param>
        /// <param name="productAttributeMappingRepository">Product attribute mapping repository</param>
        /// <param name="productAttributeCombinationRepository">Product attribute combination repository</param>
        /// <param name="productAttributeValueRepository">Product attribute value repository</param>
        /// <param name="predefinedProductAttributeValueRepository">Predefined product attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductAttributeService(ICacheManager cacheManager,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            IRepository<PredefinedProductAttributeValue> predefinedProductAttributeValueRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._productAttributeRepository = productAttributeRepository;
            this._productAttributeMappingRepository = productAttributeMappingRepository;
            this._productAttributeCombinationRepository = productAttributeCombinationRepository;
            this._productAttributeValueRepository = productAttributeValueRepository;
            this._predefinedProductAttributeValueRepository = predefinedProductAttributeValueRepository;
            this._eventPublisher = eventPublisher;
        }

        #region ProductAttribute

        public void DeleteProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Delete(productAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productAttribute);
        }

        public IPagedList<ProductAttribute> GetAllProductAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(PRODUCTATTRIBUTES_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _productAttributeRepository.Table
                            orderby pa.Name
                            select pa;
                var productAttributes = new PagedList<ProductAttribute>(query, pageIndex, pageSize);
                return productAttributes;
            });
        }

        public ProductAttribute GetProductAttributeById(int productAttributeId)
        {
            if (productAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTATTRIBUTES_BY_ID_KEY, productAttributeId);
            return _cacheManager.Get(key, () => _productAttributeRepository.GetById(productAttributeId));
        }

        public void InsertProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Insert(productAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productAttribute);
        }

        public void UpdateProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Update(productAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productAttribute);
        }

        #endregion

        #region ProductAttributeMapping

        public void DeleteProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException("productAttributeMapping");

            _productAttributeMappingRepository.Delete(productAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productAttributeMapping);
        }

        public IList<ProductAttributeMapping> GetProductAttributeMappingsByProductId(int productId)
        {
            string key = string.Format(PRODUCTATTRIBUTEMAPPINGS_ALL_KEY, productId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pam in _productAttributeMappingRepository.Table
                            orderby pam.DisplayOrder
                            where pam.ProductId == productId
                            select pam;
                var productAttributeMappings = query.ToList();
                return productAttributeMappings;
            });
        }

        public ProductAttributeMapping GetProductAttributeMappingById(int productAttributeMappingId)
        {
            if (productAttributeMappingId == 0)
                return null;

            string key = string.Format(PRODUCTATTRIBUTEMAPPINGS_BY_ID_KEY, productAttributeMappingId);
            return _cacheManager.Get(key, () => _productAttributeMappingRepository.GetById(productAttributeMappingId));
        }

        public void InsertProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException("productAttributeMapping");

            _productAttributeMappingRepository.Insert(productAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productAttributeMapping);
        }

        public void UpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException("productAttributeMapping");

            _productAttributeMappingRepository.Update(productAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productAttributeMapping);
        }

        #endregion

        #region ProductAttributeValue

        public void DeleteProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException("productAttributeValue");

            _productAttributeValueRepository.Delete(productAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productAttributeValue);
        }

        public IList<ProductAttributeValue> GetProductAttributeValues(int productAttributeMappingId)
        {
            string key = string.Format(PRODUCTATTRIBUTEVALUES_ALL_KEY, productAttributeMappingId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pav in _productAttributeValueRepository.Table
                            orderby pav.DisplayOrder
                            where pav.ProductAttributeMappingId == productAttributeMappingId
                            select pav;
                var productAttributeValues = query.ToList();
                return productAttributeValues;
            });
        }

        public ProductAttributeValue GetProductAttributeValueById(int productAttributeValueId)
        {
            if (productAttributeValueId == 0)
                return null;

            string key = string.Format(PRODUCTATTRIBUTEVALUES_BY_ID_KEY, productAttributeValueId);
            return _cacheManager.Get(key, () => _productAttributeValueRepository.GetById(productAttributeValueId));
        }

        public void InsertProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException("productAttributeValue");

            _productAttributeValueRepository.Insert(productAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productAttributeValue);
        }

        public void UpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException("productAttributeValue");

            _productAttributeValueRepository.Update(productAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productAttributeValue);
        }

        #endregion

        #region PredefinedProductAttributeValue

        public void DeletePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException("ppav");

            _predefinedProductAttributeValueRepository.Delete(ppav);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(ppav);
        }

        public IList<PredefinedProductAttributeValue> GetPredefinedProductAttributeValues(int productAttributeId)
        {
            var query = from ppav in _predefinedProductAttributeValueRepository.Table
                        orderby ppav.DisplayOrder
                        where ppav.ProductAttributeId == productAttributeId
                        select ppav;
            var values = query.ToList();
            return values;
        }

        public PredefinedProductAttributeValue GetPredefinedProductAttributeValueById(int id)
        {
            if (id == 0)
                return null;

            return _predefinedProductAttributeValueRepository.GetById(id);
        }

        public void InsertPredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException("ppav");

            _predefinedProductAttributeValueRepository.Insert(ppav);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(ppav);
        }

        public void UpdatePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException("ppav");

            _predefinedProductAttributeValueRepository.Update(ppav);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(ppav);
        }

        #endregion

        #region ProductAttributeCombination

        public void DeleteProductAttributeCombination(ProductAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productAttributeCombinationRepository.Delete(combination);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(combination);
        }

        public IList<ProductAttributeCombination> GetAllProductAttributeCombinations(int productId)
        {
            if (productId == 0)
                return new List<ProductAttributeCombination>();

            string key = string.Format(PRODUCTATTRIBUTECOMBINATIONS_ALL_KEY, productId);

            return _cacheManager.Get(key, () =>
            {
                var query = from c in _productAttributeCombinationRepository.Table
                            orderby c.Id
                            where c.ProductId == productId
                            select c;
                var combinations = query.ToList();
                return combinations;
            });
        }

        public ProductAttributeCombination GetProductAttributeCombinationById(int productAttributeCombinationId)
        {
            if (productAttributeCombinationId == 0)
                return null;

            return _productAttributeCombinationRepository.GetById(productAttributeCombinationId);
        }

        public ProductAttributeCombination GetProductAttributeCombinationBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pac in _productAttributeCombinationRepository.Table
                        orderby pac.Id
                        where pac.Sku == sku
                        select pac;
            var combination = query.FirstOrDefault();
            return combination;
        }

        public void InsertProductAttributeCombination(ProductAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productAttributeCombinationRepository.Insert(combination);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(combination);
        }

        public void UpdateProductAttributeCombination(ProductAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productAttributeCombinationRepository.Update(combination);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(combination);
        }

        #endregion

        #endregion
    }
}
