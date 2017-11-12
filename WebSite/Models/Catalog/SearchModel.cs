﻿using System.Collections.Generic;
using System.Web.Mvc;
using Web.Framework;
using Web.Framework.Mvc;

namespace WebSite.Models.Catalog
{
    public partial class SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            this.PagingFilteringContext = new CatalogPagingFilteringModel();
            this.Products = new List<ProductOverviewModel>();

            this.AvailableCategories = new List<SelectListItem>();
            this.AvailableManufacturers = new List<SelectListItem>();
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        [AllowHtml]
        public string q { get; set; }
        /// <summary>
        /// Category ID
        /// </summary>
        [NopResourceDisplayName("Search.Category")]
        public int cid { get; set; }
        [NopResourceDisplayName("Search.IncludeSubCategories")]
        public bool isc { get; set; }
        /// <summary>
        /// Manufacturer ID
        /// </summary>
        [NopResourceDisplayName("Search.Manufacturer")]
        public int mid { get; set; }
        /// <summary>
        /// Price - From 
        /// </summary>
        [AllowHtml]
        public string pf { get; set; }
        /// <summary>
        /// Price - To
        /// </summary>
        [AllowHtml]
        public string pt { get; set; }
        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [NopResourceDisplayName("Search.SearchInDescriptions")]
        public bool sid { get; set; }
        /// <summary>
        /// A value indicating whether "advanced search" is enabled
        /// </summary>
        [NopResourceDisplayName("Search.AdvancedSearch")]
        public bool adv { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }


        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductOverviewModel> Products { get; set; }

        #region Nested classes

        public class CategoryModel : BaseNopEntityModel
        {
            public string Breadcrumb { get; set; }
        }

        #endregion
    }
}