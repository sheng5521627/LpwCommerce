using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Catalog;
using Core.Data;
using Data;
using Core.Domain.Common;
using Core.Caching;
using Services.Events;
using System.Data;

namespace Services.Catalog
{
    /// <summary>
    /// Product tag service
    /// </summary>
    public partial class ProductTagService : IProductTagService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string PRODUCTTAG_COUNT_KEY = "Nop.producttag.count-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTTAG_PATTERN_KEY = "Nop.producttag.";

        #endregion

        #region Fields

        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="productTagRepository">Product tag repository</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductTagService(IRepository<ProductTag> productTagRepository,
            IDataProvider dataProvider,
            IDbContext dbContext,
            CommonSettings commonSettings,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            this._productTagRepository = productTagRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
        }

        #region Nested classes

        private class ProductTagWithCount
        {
            public int ProductTagId { get; set; }
            public int ProductCount { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get product count for each of existing product tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Dictionary of "product tag ID : product count"</returns>
        private Dictionary<int, int> GetProductCount(int storeId)
        {
            string key = string.Format(PRODUCTTAG_COUNT_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {

                if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
                {
                    //stored procedures are enabled and supported by the database. 
                    //It's much faster than the LINQ implementation below 

                    #region Use stored procedure

                    //prepare parameters
                    var pStoreId = _dataProvider.GetParameter();
                    pStoreId.ParameterName = "StoreId";
                    pStoreId.Value = storeId;
                    pStoreId.DbType = DbType.Int32;


                    //invoke stored procedure
                    var result = _dbContext.SqlQuery<ProductTagWithCount>(
                        "Exec ProductTagCountLoadAll @StoreId",
                        pStoreId);

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in result)
                        dictionary.Add(item.ProductTagId, item.ProductCount);
                    return dictionary;

                    #endregion
                }
                else
                {
                    //stored procedures aren't supported. Use LINQ
                    #region Search products
                    var query = from pt in _productTagRepository.Table
                                select new
                                {
                                    Id = pt.Id,
                                    ProductCount = pt.Products
                                        //published and not deleted products
                                        .Count(p => !p.Deleted && p.Published)
                                };

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in query)
                        dictionary.Add(item.Id, item.ProductCount);
                    return dictionary;

                    #endregion

                }
            });
        }

        #endregion

        public void DeleteProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            _productTagRepository.Delete(productTag);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productTag);
        }

        public IList<ProductTag> GetAllProductTags()
        {
            var query = _productTagRepository.Table;
            var productTags = query.ToList();
            return productTags;
        }

        public ProductTag GetProductTagById(int productTagId)
        {
            if (productTagId == 0)
                return null;

            return _productTagRepository.GetById(productTagId);
        }

        public ProductTag GetProductTagByName(string name)
        {
            var query = from pt in _productTagRepository.Table
                        where pt.Name == name
                        select pt;

            var productTag = query.FirstOrDefault();
            return productTag;
        }

        public void InsertProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            _productTagRepository.Insert(productTag);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productTag);
        }

        public void UpdateProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            _productTagRepository.Update(productTag);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productTag);
        }

        public int GetProductCount(int productTagId, int storeId)
        {
            var dictionary = GetProductCount(storeId);
            if (dictionary.ContainsKey(productTagId))
                return dictionary[productTagId];

            return 0;
        }

        #endregion
    }
}
