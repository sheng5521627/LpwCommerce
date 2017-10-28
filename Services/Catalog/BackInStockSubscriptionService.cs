﻿using Core.Data;
using Core.Domain.Catalog;
using Services.Events;
using Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Services.Common;
using Core.Domain.Customers;

namespace Services.Catalog
{
    /// <summary>
    /// Back in stock subscription service
    /// </summary>
    public partial class BackInStockSubscriptionService : IBackInStockSubscriptionService
    {
        #region Fields

        private readonly IRepository<BackInStockSubscription> _backInStockSubscriptionRepository;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="backInStockSubscriptionRepository">Back in stock subscription repository</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="eventPublisher">Event publisher</param>
        public BackInStockSubscriptionService(IRepository<BackInStockSubscription> backInStockSubscriptionRepository,
            IWorkflowMessageService workflowMessageService,
            IEventPublisher eventPublisher)
        {
            this._backInStockSubscriptionRepository = backInStockSubscriptionRepository;
            this._workflowMessageService = workflowMessageService;
            this._eventPublisher = eventPublisher;
        }

        public void DeleteSubscription(BackInStockSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _backInStockSubscriptionRepository.Delete(subscription);

            //event notification
            _eventPublisher.EntityDeleted(subscription);
        }

        public IPagedList<BackInStockSubscription> GetAllSubscriptionsByCustomerId(int customerId, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _backInStockSubscriptionRepository.Table;
            //customer
            query = query.Where(biss => biss.CustomerId == customerId);
            //store
            if (storeId > 0)
                query = query.Where(biss => biss.StoreId == storeId);
            //product
            query = query.Where(biss => !biss.Product.Deleted);
            query = query.OrderByDescending(biss => biss.CreatedOnUtc);

            return new PagedList<BackInStockSubscription>(query, pageIndex, pageSize);
        }

        public IPagedList<BackInStockSubscription> GetAllSubscriptionsByProductId(int productId, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _backInStockSubscriptionRepository.Table;
            //product
            query = query.Where(biss => biss.ProductId == productId);
            //store
            if (storeId > 0)
                query = query.Where(biss => biss.StoreId == storeId);
            //customer
            query = query.Where(biss => !biss.Customer.Deleted && biss.Customer.Active);
            query = query.OrderByDescending(biss => biss.CreatedOnUtc);
            return new PagedList<BackInStockSubscription>(query, pageIndex, pageSize);
        }

        public BackInStockSubscription FindSubscription(int customerId, int productId, int storeId)
        {
            var query = from biss in _backInStockSubscriptionRepository.Table
                        orderby biss.CreatedOnUtc descending
                        where biss.CustomerId == customerId &&
                              biss.ProductId == productId &&
                              biss.StoreId == storeId
                        select biss;

            var subscription = query.FirstOrDefault();
            return subscription;
        }

        public BackInStockSubscription GetSubscriptionById(int subscriptionId)
        {
            if (subscriptionId == 0)
                return null;

            var subscription = _backInStockSubscriptionRepository.GetById(subscriptionId);
            return subscription;
        }

        public void InsertSubscription(BackInStockSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _backInStockSubscriptionRepository.Insert(subscription);

            //event notification
            _eventPublisher.EntityInserted(subscription);
        }

        public void UpdateSubscription(BackInStockSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _backInStockSubscriptionRepository.Update(subscription);

            //event notification
            _eventPublisher.EntityUpdated(subscription);
        }

        public int SendNotificationsToSubscribers(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            int result = 0;
            var subscriptions = GetAllSubscriptionsByProductId(product.Id);
            foreach (var subscription in subscriptions)
            {
                //ensure that customer is registered (simple and fast way)
                if (CommonHelper.IsValidEmail(subscription.Customer.Email))
                {
                    var customer = subscription.Customer;
                    var customerLanguageId = customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, subscription.StoreId);
                    _workflowMessageService.SendBackInStockNotification(subscription, customerLanguageId);
                    result++;
                }
            }
            for (int i = 0; i <= subscriptions.Count - 1; i++)
                DeleteSubscription(subscriptions[i]);
            return result;
        }

        #endregion
    }
}
