using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Catalog;
using Core.Data;
using Core.Caching;
using Services.Events;

namespace Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : allow filtering
        /// {2} : show on product page
        /// </remarks>
        private const string PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.productspecificationattribute.";
        #endregion

        #region Fields

        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="specificationAttributeRepository">Specification attribute repository</param>
        /// <param name="specificationAttributeOptionRepository">Specification attribute option repository</param>
        /// <param name="productSpecificationAttributeRepository">Product specification attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public SpecificationAttributeService(ICacheManager cacheManager,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _eventPublisher = eventPublisher;
        }

        public SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            return _specificationAttributeRepository.GetById(specificationAttributeId);
        }

        public IPagedList<SpecificationAttribute> GetSpecificationAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplayOrder, sa.Id
                        select sa;
            var specificationAttributes = new PagedList<SpecificationAttribute>(query, pageIndex, pageSize);
            return specificationAttributes;
        }

        public void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Delete(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttribute);
        }

        public void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Insert(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttribute);
        }

        public void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Update(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttribute);
        }

        public SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOption)
        {
            if (specificationAttributeOption == 0)
                return null;

            return _specificationAttributeOptionRepository.GetById(specificationAttributeOption);
        }

        public IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsByIds(int[] specificationAttributeOptionIds)
        {
            if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Length == 0)
                return new List<SpecificationAttributeOption>();

            var query = from sao in _specificationAttributeOptionRepository.Table
                        where specificationAttributeOptionIds.Contains(sao.Id)
                        select sao;
            var specificationAttributeOptions = query.ToList();
            //sort by passed identifiers
            var sortedSpecificationAttributeOptions = new List<SpecificationAttributeOption>();
            foreach (int id in specificationAttributeOptionIds)
            {
                var sao = specificationAttributeOptions.Find(x => x.Id == id);
                if (sao != null)
                    sortedSpecificationAttributeOptions.Add(sao);
            }
            return sortedSpecificationAttributeOptions;
        }

        public IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();
            return specificationAttributeOptions;
        }

        public void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttributeOption);
        }

        public void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Insert(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttributeOption);
        }

        public void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Update(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttributeOption);
        }

        public void DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Delete(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productSpecificationAttribute);
        }

        public IList<ProductSpecificationAttribute> GetProductSpecificationAttributes(int productId = 0, int specificationAttributeOptionId = 0, bool? allowFiltering = default(bool?), bool? showOnProductPage = default(bool?))
        {
            string allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
            string showOnProductPageCacheStr = showOnProductPage.HasValue ? showOnProductPage.ToString() : "null";
            string key = string.Format(PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY, productId, allowFilteringCacheStr, showOnProductPageCacheStr);

            return _cacheManager.Get(key, () =>
            {
                var query = _productSpecificationAttributeRepository.Table;
                if (productId > 0)
                    query = query.Where(psa => psa.ProductId == productId);
                if (specificationAttributeOptionId > 0)
                    query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnProductPage.HasValue)
                    query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder);

                var productSpecificationAttributes = query.ToList();
                return productSpecificationAttributes;
            });
        }

        public ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;

            return _productSpecificationAttributeRepository.GetById(productSpecificationAttributeId);
        }

        public void InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Insert(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productSpecificationAttribute);
        }

        public void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Update(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productSpecificationAttribute);
        }

        public int GetProductSpecificationAttributeCount(int productId = 0, int specificationAttributeOptionId = 0)
        {
            var query = _productSpecificationAttributeRepository.Table;
            if (productId > 0)
                query = query.Where(psa => psa.ProductId == productId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return query.Count();
        }

        #endregion
    }
}
