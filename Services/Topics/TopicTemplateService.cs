using Core.Data;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Topics;

namespace Services.Topics
{
    /// <summary>
    /// Topic template service
    /// </summary>
    public partial class TopicTemplateService : ITopicTemplateService
    {
        #region Fields

        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="topicTemplateRepository">Topic template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository,
            IEventPublisher eventPublisher)
        {
            this._topicTemplateRepository = topicTemplateRepository;
            this._eventPublisher = eventPublisher;
        }

        public void DeleteTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Delete(topicTemplate);

            //event notification
            _eventPublisher.EntityDeleted(topicTemplate);
        }

        public IList<TopicTemplate> GetAllTopicTemplates()
        {
            var query = from pt in _topicTemplateRepository.Table
                        orderby pt.DisplayOrder
                        select pt;

            var templates = query.ToList();
            return templates;
        }

        public TopicTemplate GetTopicTemplateById(int topicTemplateId)
        {
            if (topicTemplateId == 0)
                return null;

            return _topicTemplateRepository.GetById(topicTemplateId);
        }

        public void InsertTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Insert(topicTemplate);

            //event notification
            _eventPublisher.EntityInserted(topicTemplate);
        }

        public void UpdateTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Update(topicTemplate);

            //event notification
            _eventPublisher.EntityUpdated(topicTemplate);
        }

        #endregion
    }
}
