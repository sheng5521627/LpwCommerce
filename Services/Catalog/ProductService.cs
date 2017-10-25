using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Catalog;
using Core.Data;
using Core.Domain.Localization;
using Core.Domain.Security;
using Core.Domain.Stores;

namespace Services.Catalog
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial class ProductService : IProductService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTS_BY_ID_KEY = "Nop.product.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        #endregion

        #region Fields

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IRepository<CrossSellProduct> _crossSellProductRepository;
        private readonly IRepository<TierPrice> _tierPriceRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="relatedProductRepository">Related product repository</param>
        /// <param name="crossSellProductRepository">Cross-sell product repository</param>
        /// <param name="tierPriceRepository">Tier price repository</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="productPictureRepository">Product picture repository</param>
        /// <param name="productSpecificationAttributeRepository">Product specification attribute repository</param>
        /// <param name="productReviewRepository">Product review repository</param>
        /// <param name="productWarehouseInventoryRepository">Product warehouse inventory repository</param>
        /// <param name="productAttributeService">Product attribute service</param>
        /// <param name="productAttributeParser">Product attribute parser service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="workContext">Work context</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        public ProductService(ICacheManager cacheManager,
            IRepository<Product> productRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<CrossSellProduct> crossSellProductRepository,
            IRepository<TierPrice> tierPriceRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IWorkContext workContext,
            LocalizationSettings localizationSettings,
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IAclService aclService,
            IStoreMappingService storeMappingService)
        {
            this._cacheManager = cacheManager;
            this._productRepository = productRepository;
            this._relatedProductRepository = relatedProductRepository;
            this._crossSellProductRepository = crossSellProductRepository;
            this._tierPriceRepository = tierPriceRepository;
            this._productPictureRepository = productPictureRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            this._productReviewRepository = productReviewRepository;
            this._productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._languageService = languageService;
            this._workflowMessageService = workflowMessageService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._localizationSettings = localizationSettings;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
        }

        public void DeleteProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public IList<Product> GetAllProductsDisplayedOnHomePage()
        {
            throw new NotImplementedException();
        }

        public Product GetProductById(int productId)
        {
            throw new NotImplementedException();
        }

        public IList<Product> GetProductsByIds(int[] productIds)
        {
            throw new NotImplementedException();
        }

        public void InsertProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public int GetCategoryProductNumber(IList<int> categoryIds = null, int storeId = 0)
        {
            throw new NotImplementedException();
        }

        public IPagedList<Product> SearchProducts(int pageIndex = 0, int pageSize = int.MaxValue, IList<int> categoryIds = null, int manufacturerId = 0, int storeId = 0, int vendorId = 0, int warehouseId = 0, ProductType? productType = default(ProductType?), bool visibleIndividuallyOnly = false, bool markedAsNewOnly = false, bool? featuredProducts = default(bool?), decimal? priceMin = default(decimal?), decimal? priceMax = default(decimal?), int productTagId = 0, string keywords = null, bool searchDescriptions = false, bool searchSku = true, bool searchProductTags = false, int languageId = 0, IList<int> filteredSpecs = null, ProductSortingEnum orderBy = ProductSortingEnum.Position, bool showHidden = false, bool? overridePublished = default(bool?))
        {
            throw new NotImplementedException();
        }

        public IPagedList<Product> SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds, bool loadFilterableSpecificationAttributeOptionIds = false, int pageIndex = 0, int pageSize = int.MaxValue, IList<int> categoryIds = null, int manufacturerId = 0, int storeId = 0, int vendorId = 0, int warehouseId = 0, ProductType? productType = default(ProductType?), bool visibleIndividuallyOnly = false, bool markedAsNewOnly = false, bool? featuredProducts = default(bool?), decimal? priceMin = default(decimal?), decimal? priceMax = default(decimal?), int productTagId = 0, string keywords = null, bool searchDescriptions = false, bool searchSku = true, bool searchProductTags = false, int languageId = 0, IList<int> filteredSpecs = null, ProductSortingEnum orderBy = ProductSortingEnum.Position, bool showHidden = false, bool? overridePublished = default(bool?))
        {
            throw new NotImplementedException();
        }

        public IPagedList<Product> GetProductsByProductAtributeId(int productAttributeId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IList<Product> GetAssociatedProducts(int parentGroupedProductId, int storeId = 0, int vendorId = 0, bool showHidden = false)
        {
            throw new NotImplementedException();
        }

        public void UpdateProductReviewTotals(Product product)
        {
            throw new NotImplementedException();
        }

        public void GetLowStockProducts(int vendorId, out IList<Product> products, out IList<ProductAttributeCombination> combinations)
        {
            throw new NotImplementedException();
        }

        public Product GetProductBySku(string sku)
        {
            throw new NotImplementedException();
        }

        public void UpdateHasTierPricesProperty(Product product)
        {
            throw new NotImplementedException();
        }

        public void UpdateHasDiscountsApplied(Product product)
        {
            throw new NotImplementedException();
        }

        public void AdjustInventory(Product product, int quantityToChange, string attributesXml = "")
        {
            throw new NotImplementedException();
        }

        public void ReserveInventory(Product product, int quantity)
        {
            throw new NotImplementedException();
        }

        public void UnblockReservedInventory(Product product, int quantity)
        {
            throw new NotImplementedException();
        }

        public void BookReservedInventory(Product product, int warehouseId, int quantity)
        {
            throw new NotImplementedException();
        }

        public int ReverseBookedInventory(Product product, ShipmentItem shipmentItem)
        {
            throw new NotImplementedException();
        }

        public void DeleteRelatedProduct(RelatedProduct relatedProduct)
        {
            throw new NotImplementedException();
        }

        public IList<RelatedProduct> GetRelatedProductsByProductId1(int productId1, bool showHidden = false)
        {
            throw new NotImplementedException();
        }

        public RelatedProduct GetRelatedProductById(int relatedProductId)
        {
            throw new NotImplementedException();
        }

        public void InsertRelatedProduct(RelatedProduct relatedProduct)
        {
            throw new NotImplementedException();
        }

        public void UpdateRelatedProduct(RelatedProduct relatedProduct)
        {
            throw new NotImplementedException();
        }

        public void DeleteCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            throw new NotImplementedException();
        }

        public IList<CrossSellProduct> GetCrossSellProductsByProductId1(int productId1, bool showHidden = false)
        {
            throw new NotImplementedException();
        }

        public CrossSellProduct GetCrossSellProductById(int crossSellProductId)
        {
            throw new NotImplementedException();
        }

        public void InsertCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            throw new NotImplementedException();
        }

        public void UpdateCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            throw new NotImplementedException();
        }

        public IList<Product> GetCrosssellProductsByShoppingCart(IList<ShoppingCartItem> cart, int numberOfProducts)
        {
            throw new NotImplementedException();
        }

        public void DeleteTierPrice(TierPrice tierPrice)
        {
            throw new NotImplementedException();
        }

        public TierPrice GetTierPriceById(int tierPriceId)
        {
            throw new NotImplementedException();
        }

        public void InsertTierPrice(TierPrice tierPrice)
        {
            throw new NotImplementedException();
        }

        public void UpdateTierPrice(TierPrice tierPrice)
        {
            throw new NotImplementedException();
        }

        public void DeleteProductPicture(ProductPicture productPicture)
        {
            throw new NotImplementedException();
        }

        public IList<ProductPicture> GetProductPicturesByProductId(int productId)
        {
            throw new NotImplementedException();
        }

        public ProductPicture GetProductPictureById(int productPictureId)
        {
            throw new NotImplementedException();
        }

        public void InsertProductPicture(ProductPicture productPicture)
        {
            throw new NotImplementedException();
        }

        public void UpdateProductPicture(ProductPicture productPicture)
        {
            throw new NotImplementedException();
        }

        public IList<ProductReview> GetAllProductReviews(int customerId, bool? approved, DateTime? fromUtc = default(DateTime?), DateTime? toUtc = default(DateTime?), string message = null)
        {
            throw new NotImplementedException();
        }

        public ProductReview GetProductReviewById(int productReviewId)
        {
            throw new NotImplementedException();
        }

        public IList<ProductReview> GetProducReviewsByIds(int[] productReviewIds)
        {
            throw new NotImplementedException();
        }

        public void DeleteProductReview(ProductReview productReview)
        {
            throw new NotImplementedException();
        }

        public void DeleteProductWarehouseInventory(ProductWarehouseInventory pwi)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
