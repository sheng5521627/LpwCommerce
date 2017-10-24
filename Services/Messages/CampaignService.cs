using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Messages;

namespace Services.Messages
{
    public partial class CampaignService : ICampaignService
    {
        public void DeleteCampaign(Campaign campaign)
        {
            throw new NotImplementedException();
        }

        public IList<Campaign> GetAllCampaigns()
        {
            throw new NotImplementedException();
        }

        public Campaign GetCampaignById(int campaignId)
        {
            throw new NotImplementedException();
        }

        public void InsertCampaign(Campaign campaign)
        {
            throw new NotImplementedException();
        }

        public void SendCampaign(Campaign campaign, EmailAccount emailAccount, string email)
        {
            throw new NotImplementedException();
        }

        public int SendCampaign(Campaign campaign, EmailAccount emailAccount, IEnumerable<NewsLetterSubscription> subscriptions)
        {
            throw new NotImplementedException();
        }

        public void UpdateCampaign(Campaign campaign)
        {
            throw new NotImplementedException();
        }
    }
}
