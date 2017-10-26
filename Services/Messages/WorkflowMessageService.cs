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
using Core.Domain.Vendors;
using Services.Localization;
using Services.Stores;
using Core;
using Services.Events;

namespace Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILanguageService _languageService;
        private readonly ITokenizer _tokenizer;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService,
            ILanguageService languageService,
            ITokenizer tokenizer,
            IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            IStoreService storeService,
            IStoreContext storeContext,
            EmailAccountSettings emailAccountSettings,
            IEventPublisher eventPublisher)
        {
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._emailAccountSettings = emailAccountSettings;
            this._eventPublisher = eventPublisher;
        }

        #region Utilities

        protected virtual int SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null)
        {
            //retrieve localized message template data
            var bcc = messageTemplate.GetLocalized(mt => mt.BccEmailAddresses, languageId);
            var subject = messageTemplate.GetLocalized(mt => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized(mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        protected virtual MessageTemplate GetActiveMessageTemplate(string messageTemplateName, int storeId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName, storeId);

            //no template found
            if (messageTemplate == null)
                return null;

            //ensure it's active
            var isActive = messageTemplate.IsActive;
            if (!isActive)
                return null;

            return messageTemplate;
        }

        protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccounId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccounId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;
        }

        protected virtual int EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }
            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");
            return language.Id;
        }

        #endregion

        public int SendCustomerRegisteredNotificationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var messageTemplate = GetActiveMessageTemplate("NewCustomer.Notification", store.Id);
            if (messageTemplate == null)
                return 0;
            var emailCount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailCount);
        }

        public int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerEmailRevalidationMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderPlacedVendorNotification(Order order, Vendor vendor, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderPaidStoreOwnerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderPaidCustomerNotification(Order order, int languageId, string attachmentFilePath = null, string attachmentFileName = null)
        {
            throw new NotImplementedException();
        }

        public int SendOrderPaidVendorNotification(Order order, Vendor vendor, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderPlacedCustomerNotification(Order order, int languageId, string attachmentFilePath = null, string attachmentFileName = null)
        {
            throw new NotImplementedException();
        }

        public int SendShipmentSentCustomerNotification(Shipment shipment, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendShipmentDeliveredCustomerNotification(Shipment shipment, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderCompletedCustomerNotification(Order order, int languageId, string attachmentFilePath = null, string attachmentFileName = null)
        {
            throw new NotImplementedException();
        }

        public int SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderRefundedStoreOwnerNotification(Order order, decimal refundedAmount, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderRefundedCustomerNotification(Order order, decimal refundedAmount, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewOrderNoteAddedCustomerNotification(OrderNote orderNote, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendRecurringPaymentCancelledStoreOwnerNotification(RecurringPayment recurringPayment, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendRecurringPaymentCancelledCustomerNotification(RecurringPayment recurringPayment, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendRecurringPaymentFailedCustomerNotification(RecurringPayment recurringPayment, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendProductEmailAFriendMessage(Customer customer, int languageId, Product product, string customerEmail, string friendsEmail, string personalMessage)
        {
            throw new NotImplementedException();
        }

        public int SendWishlistEmailAFriendMessage(Customer customer, int languageId, string customerEmail, string friendsEmail, string personalMessage)
        {
            throw new NotImplementedException();
        }

        public int SendNewReturnRequestStoreOwnerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewReturnRequestCustomerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendReturnRequestStatusChangedCustomerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewForumTopicMessage(Customer customer, ForumTopic forumTopic, Forum forum, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewForumPostMessage(Customer customer, ForumPost forumPost, ForumTopic forumTopic, Forum forum, int friendlyForumTopicPageIndex, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendPrivateMessageNotification(PrivateMessage privateMessage, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewVendorAccountApplyStoreOwnerNotification(Customer customer, Vendor vendor, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendVendorInformationChangeNotification(Vendor vendor, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendProductReviewNotificationMessage(ProductReview productReview, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendQuantityBelowStoreOwnerNotification(Product product, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendQuantityBelowStoreOwnerNotification(ProductAttributeCombination combination, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewVatSubmittedStoreOwnerNotification(Customer customer, string vatName, string vatAddress, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendBackInStockNotification(BackInStockSubscription subscription, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendContactUsMessage(int languageId, string senderEmail, string senderName, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public int SendContactVendorMessage(Vendor vendor, int languageId, string senderEmail, string senderName, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public int SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNotification(MessageTemplate messageTemplate, EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens, string toEmailAddress, string toName, string attachmentFilePath = null, string attachmentFileName = null, string replyToEmailAddress = null, string replyToName = null, string fromEmail = null, string fromName = null, string subject = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
