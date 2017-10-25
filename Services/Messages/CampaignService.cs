using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Messages;
using Core.Data;

namespace Services.Messages
{
    public partial class CampaignService : ICampaignService
    {
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;

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
