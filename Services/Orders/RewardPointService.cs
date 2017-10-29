using Core;
using Core.Data;
using Core.Domain.Customers;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Orders;

namespace Services.Orders
{
    /// <summary>
    /// Reward point service
    /// </summary>
    public partial class RewardPointService : IRewardPointService
    {
        #region Fields

        private readonly IRepository<RewardPointsHistory> _rphRepository;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rphRepository">RewardPointsHistory repository</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event published</param>
        public RewardPointService(IRepository<RewardPointsHistory> rphRepository,
            RewardPointsSettings rewardPointsSettings,
            IStoreContext storeContext,
            IEventPublisher eventPublisher)
        {
            this._rphRepository = rphRepository;
            this._rewardPointsSettings = rewardPointsSettings;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
        }

        public IList<RewardPointsHistory> GetRewardPointsHistory(int customerId = 0, bool showHidden = false)
        {
            var query = _rphRepository.Table;
            if (customerId > 0)
                query = query.Where(rph => rph.CustomerId == customerId);
            if (!showHidden && !_rewardPointsSettings.PointsAccumulatedForAllStores)
            {
                //filter by store
                var currentStoreId = _storeContext.CurrentStore.Id;
                query = query.Where(rph => rph.StoreId == currentStoreId);
            }
            query = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id);

            var records = query.ToList();
            return records;
        }

        public void AddRewardPointsHistoryEntry(Customer customer, int points, int storeId, string message = "", Order usedWithOrder = null, decimal usedAmount = 0)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (storeId <= 0)
                throw new ArgumentException("Store ID should be valid");

            var rph = new RewardPointsHistory
            {
                Customer = customer,
                StoreId = storeId,
                UsedWithOrder = usedWithOrder,
                Points = points,
                PointsBalance = GetRewardPointsBalance(customer.Id, storeId) + points,
                UsedAmount = usedAmount,
                Message = message,
                CreatedOnUtc = DateTime.UtcNow
            };

            _rphRepository.Insert(rph);

            //event notification
            _eventPublisher.EntityInserted(rph);
        }

        public int GetRewardPointsBalance(int customerId, int storeId)
        {
            var query = _rphRepository.Table;
            if (customerId > 0)
                query = query.Where(rph => rph.CustomerId == customerId);
            if (!_rewardPointsSettings.PointsAccumulatedForAllStores)
                query = query.Where(rph => rph.StoreId == storeId);
            query = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id);

            var lastRph = query.FirstOrDefault();
            return lastRph != null ? lastRph.PointsBalance.HasValue ? lastRph.PointsBalance.Value : 0 : 0;
        }

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>Reward point history entry</returns>
        public virtual RewardPointsHistory GetRewardPointsHistoryEntryById(int rewardPointsHistoryId)
        {
            if (rewardPointsHistoryId == 0)
                return null;

            return _rphRepository.GetById(rewardPointsHistoryId);
        }

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public virtual void DeleteRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException("rewardPointsHistory");

            _rphRepository.Delete(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityDeleted(rewardPointsHistory);
        }

        /// <summary>
        /// Updates the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public virtual void UpdateRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException("rewardPointsHistory");

            _rphRepository.Update(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityUpdated(rewardPointsHistory);
        }

        /// <summary>
        /// Add reward points history record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="points">Number of points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">The order for which points were redeemed (spent) as a payment</param>
        /// <param name="usedAmount">Used amount</param>
        /// <param name="activatingDate">Date and time of activating reward points; pass null to immediately activating</param>
        /// <returns>Reward points history entry identifier</returns>
        public virtual int AddRewardPointsHistoryEntryNews(Customer customer, int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (storeId <= 0)
                throw new ArgumentException("Store ID should be valid");

            var rph = new RewardPointsHistory
            {
                Customer = customer,
                StoreId = storeId,
                UsedWithOrder = usedWithOrder,
                Points = points,
                PointsBalance = activatingDate.HasValue ? null : (int?)(GetRewardPointsBalance(customer.Id, storeId) + points),
                UsedAmount = usedAmount,
                Message = message,
                CreatedOnUtc = activatingDate ?? DateTime.UtcNow
            };

            _rphRepository.Insert(rph);

            //event notification
            _eventPublisher.EntityInserted(rph);

            return rph.Id;
        }
        #endregion
    }
}
