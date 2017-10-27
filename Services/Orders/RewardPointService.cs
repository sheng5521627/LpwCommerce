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
            return lastRph != null ? lastRph.PointsBalance : 0;
        }

        #endregion
    }
}
