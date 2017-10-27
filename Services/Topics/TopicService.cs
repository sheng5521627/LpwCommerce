using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Topics;
using Core.Data;
using Core.Domain.Stores;
using Services.Stores;
using Core;
using Core.Domain.Security;
using Core.Domain.Catalog;
using Services.Events;
using Core.Caching;
using Services.Customers;

namespace Services.Topics
{
    /// <summary>
    /// Topic service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : ignore ACL?
        /// </remarks>
        private const string TOPICS_ALL_KEY = "Nop.topics.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic ID
        /// </remarks>
        private const string TOPICS_BY_ID_KEY = "Nop.topics.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string TOPICS_PATTERN_KEY = "Nop.topics.";

        #endregion

        #region Fields

        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public TopicService(IRepository<Topic> topicRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            IRepository<AclRecord> aclRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._topicRepository = topicRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._storeMappingService = storeMappingService;
            this._workContext = workContext;
            this._aclRepository = aclRepository;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        public void DeleteTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Delete(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(topic);
        }

        public Topic GetTopicById(int topicId)
        {
            if (topicId == 0)
                return null;

            string key = string.Format(TOPICS_BY_ID_KEY, topicId);
            return _cacheManager.Get(key, () => _topicRepository.GetById(topicId));
        }

        public Topic GetTopicBySystemName(string systemName, int storeId = 0)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;
            var query = _topicRepository.Table;
            query = query.Where(m => m.SystemName == systemName);
            query = query.OrderBy(m => m.DisplayOrder);
            var topics = query.ToList();
            if (storeId > 0)
            {
                topics = topics.Where(m => _storeMappingService.Authorize(m)).ToList();
            }

            return topics.FirstOrDefault();
        }

        public IList<Topic> GetAllTopics(int storeId, bool ignorAcl = false)
        {
            string key = string.Format(TOPICS_ALL_KEY, storeId, ignorAcl);
            return _cacheManager.Get(key, () => 
            {
                var query = _topicRepository.Table;
                query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.SystemName);

                if((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!ignorAcl && !_catalogSettings.IgnoreAcl))
                {
                    if(!ignorAcl && !_catalogSettings.IgnoreAcl)
                    {
                        var allowedCustomerRoleIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from t in query
                                join acl in _aclRepository.Table
                                on new { c1 = t.Id, c2 = "Topic" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into t_acl
                                from acl in t_acl.DefaultIfEmpty()
                                where !t.SubjectToAcl || allowedCustomerRoleIds.Contains(acl.CustomerRoleId)
                                select t;
                    }

                    if(storeId>0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Topic" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }

                    query = from t in query
                            group t by t.Id into g
                            orderby g.Key
                            select g.FirstOrDefault();
                    query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.SystemName);
                }
                return query.ToList();
            });
        }

        public void InsertTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Insert(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(topic);
        }

        public void UpdateTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Update(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(topic);
        }

        #endregion
    }
}
