﻿using Core.Domain.Catalog;
using Services.Localization;
using Services.Security;
using Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Catalog
{
    public static class CategoryExtensions
    {
        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public static IList<Category> SortCategoriesForTress(this IList<Category> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<Category>();
            foreach (var cat in source.Where(m => m.ParentCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortCategoriesForTress(source, cat.Id, true));
            }
            if (!ignoreCategoriesWithoutExistingParent && result.Count != source.Count)
            {
                // find categories without parent in provided category source and insert them into result
                foreach (var cat in source)
                {
                    if (result.FirstOrDefault(m => m.Id == cat.Id) == null)
                        result.Add(cat);
                }
            }

            return result;
        }

        public static ProductCategory FindProductCategory(this IList<ProductCategory> source, int productId, int categoryId)
        {
            foreach (var productCategory in source)
            {
                if (productCategory.ProductId == productId && productCategory.CategoryId == categoryId)
                    return productCategory;
            }
            return null;
        }

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="categoryService">Category service</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public static string GetFormattedBreadCrumb(this Category category,
            ICategoryService categoryService,
            string separator = ">>", int languageId = 0)
        {
            string result = string.Empty;
            var breakCrumb = GetCategoryBreadCrumb(category, categoryService, null, null, true);
            for (int i = 0; i <= breakCrumb.Count - 1; i++)
            {
                var categoryName = breakCrumb[i].GetLocalized(m => m.Name, languageId);
                result = string.IsNullOrEmpty(result) ? categoryName : string.Format("{0} {1} {2}", result, separator, categoryName);
            }
            return result;
        }

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public static string GetFormattedBreadCrumb(this Category category,
            IList<Category> allCategories,
            string separator = ">>", int languageId = 0)
        {
            string result = string.Empty;

            var breadcrumb = GetCategoryBreadCrumb(category, allCategories, null, null, true);
            for (int i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = breadcrumb[i].GetLocalized(x => x.Name, languageId);
                result = String.IsNullOrEmpty(result)
                    ? categoryName
                    : string.Format("{0} {1} {2}", result, separator, categoryName);
            }

            return result;
        }

        /// <summary>
        /// Get category breadcrumb (面包屑)
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="categoryService">Category service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        public static IList<Category> GetCategoryBreadCrumb(this Category category,
            ICategoryService categoryService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var result = new List<Category>();
            var alreadyProcessedCategoryIds = new List<int>();

            while (category != null &&
                  !category.Deleted &&
                  (showHidden || category.Published) &&
                  (showHidden || aclService.Authorize(category)) &&
                  (showHidden || storeMappingService.Authorize(category)) &&
                  !alreadyProcessedCategoryIds.Contains(category.Id))
            {
                result.Add(category);
                alreadyProcessedCategoryIds.Add(category.Id);
                category = categoryService.GetCategoryById(category.ParentCategoryId);
            }
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        public static IList<Category> GetCategoryBreadCrumb(this Category category,
            IList<Category> allCategories,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var result = new List<Category>();

            //used to prevent circular references
            var alreadyProcessedCategoryIds = new List<int>();

            while (category != null && //not null
                !category.Deleted && //not deleted
                (showHidden || category.Published) && //published
                (showHidden || aclService.Authorize(category)) && //ACL
                (showHidden || storeMappingService.Authorize(category)) && //Store mapping
                !alreadyProcessedCategoryIds.Contains(category.Id)) //prevent circular references
            {
                result.Add(category);

                alreadyProcessedCategoryIds.Add(category.Id);

                category = (from c in allCategories
                            where c.Id == category.ParentCategoryId
                            select c).FirstOrDefault();
            }
            result.Reverse();
            return result;
        }
    }
}
