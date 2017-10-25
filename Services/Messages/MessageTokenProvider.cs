using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Blogs;
using Core.Domain.Catalog;
using Core.Domain.Customers;
using Core.Domain.Forums;
using Core.Domain.Messages;
using Core.Domain.News;
using Core.Domain.Orders;
using Core.Domain.Shipping;
using Core.Domain.Stores;
using Core.Domain.Vendors;
using Services.Localization;
using Services.Helpers;
using Services.Directory;
using Core;
using Services.Media;
using Services.Catalog;

namespace Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        private readonly IDownloadService _downloadService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;

        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ShippingSettings _shippingSettings;

        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            ICurrencyService currencyService,
            IWorkContext workContext,
            IDownloadService downloadService,
            IOrderService orderService,
            IPaymentService paymentService,
            IStoreService storeService,
            IStoreContext storeContext,
            IProductAttributeParser productAttributeParser,
            IAddressAttributeFormatter addressAttributeFormatter,
            MessageTemplatesSettings templatesSettings,
            CatalogSettings catalogSettings,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            ShippingSettings shippingSettings,
            IEventPublisher eventPublisher)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._workContext = workContext;
            this._downloadService = downloadService;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._productAttributeParser = productAttributeParser;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._storeService = storeService;
            this._storeContext = storeContext;

            this._templatesSettings = templatesSettings;
            this._catalogSettings = catalogSettings;
            this._taxSettings = taxSettings;
            this._currencySettings = currencySettings;
            this._shippingSettings = shippingSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void AddAttributeCombinationTokens(IList<Token> tokens, ProductAttributeCombination combination, int languageId)
        {
            throw new NotImplementedException();
        }

        public void AddBackInStockTokens(IList<Token> tokens, BackInStockSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment)
        {
            throw new NotImplementedException();
        }

        public void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            throw new NotImplementedException();
        }

        public void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost)
        {
            throw new NotImplementedException();
        }

        public void AddForumTokens(IList<Token> tokens, Forum forum)
        {
            throw new NotImplementedException();
        }

        public void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic, int? friendlyForumTopicPageIndex = default(int?), int? appendedPostIdentifierAnchor = default(int?))
        {
            throw new NotImplementedException();
        }

        public void AddGiftCardTokens(IList<Token> tokens, GiftCard giftCard)
        {
            throw new NotImplementedException();
        }

        public void AddNewsCommentTokens(IList<Token> tokens, NewsComment newsComment)
        {
            throw new NotImplementedException();
        }

        public void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public void AddOrderNoteTokens(IList<Token> tokens, OrderNote orderNote)
        {
            throw new NotImplementedException();
        }

        public void AddOrderRefundedTokens(IList<Token> tokens, Order order, decimal refundedAmount)
        {
            throw new NotImplementedException();
        }

        public void AddOrderTokens(IList<Token> tokens, Order order, int languageId, int vendorId = 0)
        {
            throw new NotImplementedException();
        }

        public void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage)
        {
            throw new NotImplementedException();
        }

        public void AddProductReviewTokens(IList<Token> tokens, ProductReview productReview)
        {
            throw new NotImplementedException();
        }

        public void AddProductTokens(IList<Token> tokens, Product product, int languageId)
        {
            throw new NotImplementedException();
        }

        public void AddRecurringPaymentTokens(IList<Token> tokens, RecurringPayment recurringPayment)
        {
            throw new NotImplementedException();
        }

        public void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        public void AddShipmentTokens(IList<Token> tokens, Shipment shipment, int languageId)
        {
            throw new NotImplementedException();
        }

        public void AddStoreTokens(IList<Token> tokens, Store store, EmailAccount emailAccount)
        {
            throw new NotImplementedException();
        }

        public void AddVendorTokens(IList<Token> tokens, Vendor vendor)
        {
            throw new NotImplementedException();
        }

        public string[] GetListOfAllowedTokens()
        {
            throw new NotImplementedException();
        }

        public string[] GetListOfCampaignAllowedTokens()
        {
            throw new NotImplementedException();
        }
    }
}
