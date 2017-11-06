using System;
using System.Collections.Generic;
using Web.Framework;
using Web.Framework.Mvc;

namespace WebSite.Models.Order
{
    public partial class CustomerRewardPointsModel : BaseNopModel
    {
        public CustomerRewardPointsModel()
        {
            RewardPoints = new List<RewardPointsHistoryModel>();
        }

        public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
        public int RewardPointsBalance { get; set; }
        public string RewardPointsAmount { get; set; }
        public int MinimumRewardPointsBalance { get; set; }
        public string MinimumRewardPointsAmount { get; set; }

        #region Nested classes

        public partial class RewardPointsHistoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [NopResourceDisplayName("RewardPoints.Fields.PointsBalance")]
            public int PointsBalance { get; set; }

            [NopResourceDisplayName("RewardPoints.Fields.Message")]
            public string Message { get; set; }

            [NopResourceDisplayName("RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}