using Core.Domain.Customers;
using Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Orders
{
    /// <summary>
    /// Reward point service interface 奖励点服务接口
    /// </summary>
    public partial interface IRewardPointService
    {
        /// <summary>
        /// Load reward point history records
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records (filter by current store if possible)</param>
        /// <returns>Reward point history records</returns>
        IList<RewardPointsHistory> GetRewardPointsHistory(int customerId = 0, bool showHidden = false);

        /// <summary>
        /// Add reward points history record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="points">Number of points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">the order for which points were redeemed as a payment</param>
        /// <param name="usedAmount">Used amount</param>
        void AddRewardPointsHistoryEntry(Customer customer,
            int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M);

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier; pass </param>
        /// <returns>Balance</returns>
        int GetRewardPointsBalance(int customerId, int storeId);

        void UpdateRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>Reward point history entry</returns>
        RewardPointsHistory GetRewardPointsHistoryEntryById(int rewardPointsHistoryId);

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        void DeleteRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);

        int AddRewardPointsHistoryEntryNews(Customer customer,
            int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null);
    }
}
