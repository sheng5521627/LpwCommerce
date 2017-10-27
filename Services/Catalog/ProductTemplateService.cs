using Core.Data;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Catalog;

namespace Services.Catalog
{
    /// <summary>
    /// Product template service
    /// </summary>
    public partial class ProductTemplateService : IProductTemplateService
    {
        #region Fields

        private readonly IRepository<ProductTemplate> _productTemplateRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="productTemplateRepository">Product template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductTemplateService(IRepository<ProductTemplate> productTemplateRepository,
            IEventPublisher eventPublisher)
        {
            this._productTemplateRepository = productTemplateRepository;
            this._eventPublisher = eventPublisher;
        }

        public void DeleteProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            _productTemplateRepository.Delete(productTemplate);

            //event notification
            _eventPublisher.EntityDeleted(productTemplate);
        }

        public IList<ProductTemplate> GetAllProductTemplates()
        {
            var query = from pt in _productTemplateRepository.Table
                        orderby pt.DisplayOrder
                        select pt;

            var templates = query.ToList();
            return templates;
        }

        public ProductTemplate GetProductTemplateById(int productTemplateId)
        {
            if (productTemplateId == 0)
                return null;

            return _productTemplateRepository.GetById(productTemplateId);
        }

        public void InsertProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            _productTemplateRepository.Insert(productTemplate);

            //event notification
            _eventPublisher.EntityInserted(productTemplate);
        }

        public void UpdateProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            _productTemplateRepository.Update(productTemplate);

            //event notification
            _eventPublisher.EntityUpdated(productTemplate);
        }

        #endregion
    }
}
