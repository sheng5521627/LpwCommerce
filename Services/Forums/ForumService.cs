using Core;
using Core.Caching;
using Core.Data;
using Core.Domain.Customers;
using Core.Domain.Forums;
using Services.Common;
using Services.Customers;
using Services.Events;
using Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Forums
{
    /// <summary>
    /// Forum service
    /// </summary>
    public partial class ForumService : IForumService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string FORUMGROUP_ALL_KEY = "Nop.forumgroup.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : forum group ID
        /// </remarks>
        private const string FORUM_ALLBYFORUMGROUPID_KEY = "Nop.forum.allbyforumgroupid-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string FORUMGROUP_PATTERN_KEY = "Nop.forumgroup.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string FORUM_PATTERN_KEY = "Nop.forum.";

        #endregion

        #region Fields
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<PrivateMessage> _forumPrivateMessageRepository;
        private readonly IRepository<ForumSubscription> _forumSubscriptionRepository;
        private readonly ForumSettings _forumSettings;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="forumGroupRepository">Forum group repository</param>
        /// <param name="forumRepository">Forum repository</param>
        /// <param name="forumTopicRepository">Forum topic repository</param>
        /// <param name="forumPostRepository">Forum post repository</param>
        /// <param name="forumPrivateMessageRepository">Private message repository</param>
        /// <param name="forumSubscriptionRepository">Forum subscription repository</param>
        /// <param name="forumSettings">Forum settings</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="eventPublisher">Event published</param>
        public ForumService(ICacheManager cacheManager,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<Forum> forumRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<PrivateMessage> forumPrivateMessageRepository,
            IRepository<ForumSubscription> forumSubscriptionRepository,
            ForumSettings forumSettings,
            IRepository<Customer> customerRepository,
            IGenericAttributeService genericAttributeService,
            ICustomerService customerService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IEventPublisher eventPublisher
            )
        {
            this._cacheManager = cacheManager;
            this._forumGroupRepository = forumGroupRepository;
            this._forumRepository = forumRepository;
            this._forumTopicRepository = forumTopicRepository;
            this._forumPostRepository = forumPostRepository;
            this._forumPrivateMessageRepository = forumPrivateMessageRepository;
            this._forumSubscriptionRepository = forumSubscriptionRepository;
            this._forumSettings = forumSettings;
            this._customerRepository = customerRepository;
            this._genericAttributeService = genericAttributeService;
            this._customerService = customerService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            _eventPublisher = eventPublisher;
        }

        #region Utilities

        private void UpdateForumStats(int forumId)
        {
            if (forumId == 0)
            {
                return;
            }
            var forum = GetForumById(forumId);
            if (forum == null)
            {
                return;
            }

            //number of topics
            var queryNumTopics = from ft in _forumTopicRepository.Table
                                 where ft.ForumId == forumId
                                 select ft.Id;
            int numTopics = queryNumTopics.Count();

            //number of posts
            var queryNumPosts = from ft in _forumTopicRepository.Table
                                join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                                where ft.ForumId == forumId
                                select fp.Id;
            int numPosts = queryNumPosts.Count();

            //last values
            int lastTopicId = 0;
            int lastPostId = 0;
            int lastPostCustomerId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from ft in _forumTopicRepository.Table
                                  join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                                  where ft.ForumId == forumId
                                  orderby fp.CreatedOnUtc descending, ft.CreatedOnUtc descending
                                  select new
                                  {
                                      LastTopicId = ft.Id,
                                      LastPostId = fp.Id,
                                      LastPostCustomerId = fp.CustomerId,
                                      LastPostTime = fp.CreatedOnUtc
                                  };
            var lastValues = queryLastValues.FirstOrDefault();
            if (lastValues != null)
            {
                lastTopicId = lastValues.LastTopicId;
                lastPostId = lastValues.LastPostId;
                lastPostCustomerId = lastValues.LastPostCustomerId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update forum
            forum.NumTopics = numTopics;
            forum.NumPosts = numPosts;
            forum.LastTopicId = lastTopicId;
            forum.LastPostId = lastPostId;
            forum.LastPostCustomerId = lastPostCustomerId;
            forum.LastPostTime = lastPostTime;
            UpdateForum(forum);
        }

        /// <summary>
        /// Update forum topic stats
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        private void UpdateForumTopicStats(int forumTopicId)
        {
            if (forumTopicId == 0)
            {
                return;
            }
            var forumTopic = GetTopicById(forumTopicId);
            if (forumTopic == null)
            {
                return;
            }

            //number of posts
            var queryNumPosts = from fp in _forumPostRepository.Table
                                where fp.TopicId == forumTopicId
                                select fp.Id;
            int numPosts = queryNumPosts.Count();

            //last values
            int lastPostId = 0;
            int lastPostCustomerId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from fp in _forumPostRepository.Table
                                  where fp.TopicId == forumTopicId
                                  orderby fp.CreatedOnUtc descending
                                  select new
                                  {
                                      LastPostId = fp.Id,
                                      LastPostCustomerId = fp.CustomerId,
                                      LastPostTime = fp.CreatedOnUtc
                                  };
            var lastValues = queryLastValues.FirstOrDefault();
            if (lastValues != null)
            {
                lastPostId = lastValues.LastPostId;
                lastPostCustomerId = lastValues.LastPostCustomerId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update topic
            forumTopic.NumPosts = numPosts;
            forumTopic.LastPostId = lastPostId;
            forumTopic.LastPostCustomerId = lastPostCustomerId;
            forumTopic.LastPostTime = lastPostTime;
            UpdateTopic(forumTopic);
        }

        /// <summary>
        /// Update customer stats
        /// </summary>
        /// <param name="customerId">The customer identifier</param>
        private void UpdateCustomerStats(int customerId)
        {
            if (customerId == 0)
            {
                return;
            }

            var customer = _customerService.GetCustomerById(customerId);

            if (customer == null)
            {
                return;
            }

            var query = from fp in _forumPostRepository.Table
                        where fp.CustomerId == customerId
                        select fp.Id;
            int numPosts = query.Count();

            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ForumPostCount, numPosts);
        }

        private bool IsForumModerator(Customer customer)
        {
            if (customer.IsForumModerator())
            {
                return true;
            }
            return false;
        }

        #endregion

        public void DeleteForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            _forumGroupRepository.Delete(forumGroup);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(forumGroup);
        }

        public ForumGroup GetForumGroupById(int forumGroupId)
        {
            if (forumGroupId == 0)
            {
                return null;
            }

            return _forumGroupRepository.GetById(forumGroupId);
        }

        public IList<ForumGroup> GetAllForumGroups()
        {
            string key = string.Format(FORUMGROUP_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from fg in _forumGroupRepository.Table
                            orderby fg.DisplayOrder
                            select fg;
                return query.ToList();
            });
        }

        public void InsertForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            _forumGroupRepository.Insert(forumGroup);

            //cache
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(forumGroup);
        }

        public void UpdateForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            _forumGroupRepository.Update(forumGroup);

            //cache
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(forumGroup);
        }

        public void DeleteForum(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            //delete forum subscriptions (topics)
            var queryTopicIds = from ft in _forumTopicRepository.Table
                                where ft.ForumId == forum.Id
                                select ft.Id;
            var queryFs1 = from fs in _forumSubscriptionRepository.Table
                           where queryTopicIds.Contains(fs.TopicId)
                           select fs;
            foreach (var fs in queryFs1.ToList())
            {
                _forumSubscriptionRepository.Delete(fs);
                //event notification
                _eventPublisher.EntityDeleted(fs);
            }

            //delete forum subscriptions (forum)
            var queryFs2 = from fs in _forumSubscriptionRepository.Table
                           where fs.ForumId == forum.Id
                           select fs;
            foreach (var fs2 in queryFs2.ToList())
            {
                _forumSubscriptionRepository.Delete(fs2);
                //event notification
                _eventPublisher.EntityDeleted(fs2);
            }

            //delete forum
            _forumRepository.Delete(forum);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(forum);
        }

        public Forum GetForumById(int forumId)
        {
            throw new NotImplementedException();
        }

        public IList<Forum> GetAllForumsByGroupId(int forumGroupId)
        {
            throw new NotImplementedException();
        }

        public void InsertForum(Forum forum)
        {
            throw new NotImplementedException();
        }

        public void UpdateForum(Forum forum)
        {
            throw new NotImplementedException();
        }

        public void DeleteTopic(ForumTopic forumTopic)
        {
            throw new NotImplementedException();
        }

        public ForumTopic GetTopicById(int forumTopicId)
        {
            throw new NotImplementedException();
        }

        public ForumTopic GetTopicById(int forumTopicId, bool increaseViews)
        {
            throw new NotImplementedException();
        }

        public IPagedList<ForumTopic> GetAllTopics(int forumId = 0, int customerId = 0, string keywords = "", ForumSearchType searchType = ForumSearchType.All, int limitDays = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IPagedList<ForumTopic> GetActiveTopics(int forumId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public void InsertTopic(ForumTopic forumTopic, bool sendNotifications)
        {
            throw new NotImplementedException();
        }

        public void UpdateTopic(ForumTopic forumTopic)
        {
            throw new NotImplementedException();
        }

        public ForumTopic MoveTopic(int forumTopicId, int newForumId)
        {
            throw new NotImplementedException();
        }

        public void DeletePost(ForumPost forumPost)
        {
            throw new NotImplementedException();
        }

        public ForumPost GetPostById(int forumPostId)
        {
            throw new NotImplementedException();
        }

        public IPagedList<ForumPost> GetAllPosts(int forumTopicId = 0, int customerId = 0, string keywords = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IPagedList<ForumPost> GetAllPosts(int forumTopicId = 0, int customerId = 0, string keywords = "", bool ascSort = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public void InsertPost(ForumPost forumPost, bool sendNotifications)
        {
            throw new NotImplementedException();
        }

        public void UpdatePost(ForumPost forumPost)
        {
            throw new NotImplementedException();
        }

        public void DeletePrivateMessage(PrivateMessage privateMessage)
        {
            throw new NotImplementedException();
        }

        public PrivateMessage GetPrivateMessageById(int privateMessageId)
        {
            throw new NotImplementedException();
        }

        public IPagedList<PrivateMessage> GetAllPrivateMessages(int storeId, int fromCustomerId, int toCustomerId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient, string keywords, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public void InsertPrivateMessage(PrivateMessage privateMessage)
        {
            throw new NotImplementedException();
        }

        public void UpdatePrivateMessage(PrivateMessage privateMessage)
        {
            throw new NotImplementedException();
        }

        public void DeleteSubscription(ForumSubscription forumSubscription)
        {
            throw new NotImplementedException();
        }

        public ForumSubscription GetSubscriptionById(int forumSubscriptionId)
        {
            throw new NotImplementedException();
        }

        public IPagedList<ForumSubscription> GetAllSubscriptions(int customerId = 0, int forumId = 0, int topicId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public void InsertSubscription(ForumSubscription forumSubscription)
        {
            throw new NotImplementedException();
        }

        public void UpdateSubscription(ForumSubscription forumSubscription)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToCreateTopic(Customer customer, Forum forum)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToEditTopic(Customer customer, ForumTopic topic)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToMoveTopic(Customer customer, ForumTopic topic)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToDeleteTopic(Customer customer, ForumTopic topic)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToCreatePost(Customer customer, ForumTopic topic)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToEditPost(Customer customer, ForumPost post)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToDeletePost(Customer customer, ForumPost post)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToSetTopicPriority(Customer customer)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomerAllowedToSubscribe(Customer customer)
        {
            throw new NotImplementedException();
        }

        public int CalculateTopicPageIndex(int forumTopicId, int pageSize, int postId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
