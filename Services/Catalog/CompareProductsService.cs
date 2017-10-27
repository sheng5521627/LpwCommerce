using Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services.Catalog
{
    /// <summary>
    /// Compare products service
    /// </summary>
    public partial class CompareProductsService : ICompareProductsService
    {
        #region Constants

        /// <summary>
        /// Compare products cookie name
        /// </summary>
        private const string COMPARE_PRODUCTS_COOKIE_NAME = "nop.CompareProducts";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="productService">Product service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public CompareProductsService(HttpContextBase httpContext, IProductService productService,
            CatalogSettings catalogSettings)
        {
            this._httpContext = httpContext;
            this._productService = productService;
            this._catalogSettings = catalogSettings;
        }

        #region Utilities

        protected virtual List<int> GetComparedProductIds()
        {
            var productIds = new List<int>();
            HttpCookie comparedCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (comparedCookie == null)
                return productIds;

            string[] values = comparedCookie.Values.GetValues("CompareProductIds");
            if (values == null)
                return productIds;


            foreach (string productId in values)
            {
                int id = int.Parse(productId);
                if (!productIds.Contains(id))
                    productIds.Add(id);
            }

            return productIds;
        }

        #endregion

        public void ClearCompareProducts()
        {
            HttpCookie comparedCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (comparedCookie != null)
            {
                comparedCookie.Values.Clear();
                comparedCookie.Expires.AddYears(-1);
                _httpContext.Response.SetCookie(comparedCookie);
            }
        }

        public IList<Product> GetComparedProducts()
        {
            var products = new List<Product>();
            var productIds = GetComparedProductIds();
            foreach(var productId in productIds)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && !product.Deleted && product.Published)
                    products.Add(product);
            }

            return products;
        }

        public void RemoveProductFromCompareList(int productId)
        {
            var oldProductIds = GetComparedProductIds();
            var newProductIds = new List<int>();
            newProductIds.AddRange(oldProductIds);
            newProductIds.Remove(productId);

            var comparedCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (comparedCookie == null)
                return;
            comparedCookie.Values.Clear();
            foreach(var newProductId in newProductIds)
            {
                comparedCookie.Values.Add("CompareProductIds", newProductId.ToString());
            }
            comparedCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.SetCookie(comparedCookie);
        }

        public void AddProductToCompareList(int productId)
        {
            var oldProductIds = GetComparedProductIds();
            var newProductIds = new List<int>();
            newProductIds.Add(productId); 
            foreach(var oldProductId in oldProductIds)
            {
                if (oldProductId != productId)
                {
                    newProductIds.Add(oldProductId);
                }
            }
            var comparedCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if(comparedCookie == null)
            {
                comparedCookie = new HttpCookie(COMPARE_PRODUCTS_COOKIE_NAME);
                comparedCookie.HttpOnly = true;
            }
            comparedCookie.Values.Clear();
            int i = 1;
            foreach(var newProductId in newProductIds)
            {
                comparedCookie.Values.Add("CompareProductIds", newProductId.ToString());
                if (i == _catalogSettings.CompareProductsNumber)
                    break;
                i++;
            }
            comparedCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.SetCookie(comparedCookie);
        }

        #endregion
    }
}
