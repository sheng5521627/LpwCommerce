using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Data;
using Core.Caching;
using Services.Events;
using Core;
using Data;

namespace Services.Common
{
    public partial class GenericAttributeService : IGenericAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : key group
        /// </remarks>
        private const string GENERICATTRIBUTE_KEY = "Nop.genericattribute.{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string GENERICATTRIBUTE_PATTERN_KEY = "Nop.genericattribute.";
        #endregion

        #region Fields

        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="genericAttributeRepository">Generic attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public GenericAttributeService(ICacheManager cacheManager,
            IRepository<GenericAttribute> genericAttributeRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._genericAttributeRepository = genericAttributeRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void DeleteAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentException("attibute");

            _genericAttributeRepository.Delete(attribute);

            _eventPublisher.EntityDeleted(attribute);

            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);
        }

        public GenericAttribute GetAttributeById(int attributeId)
        {
            if (attributeId == 0)
                return null;

            return _genericAttributeRepository.GetById(attributeId);
        }

        public IList<GenericAttribute> GetAttributesForEntity(int entityId, string keyGroup)
        {
            string key = string.Format(GENERICATTRIBUTE_KEY, entityId, keyGroup);
            return _cacheManager.Get(key, () => 
            {
                var query = from g in _genericAttributeRepository.Table
                            where g.EntityId == entityId && g.KeyGroup == keyGroup
                            select g;
                return query.ToList();
            });
        }

        public void InsertAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentException("attibute");

            _genericAttributeRepository.Insert(attribute);

            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            _eventPublisher.EntityInserted(attribute);
        }

        public void SaveAttribute<TPropType>(BaseEntity entity, string key, TPropType value, int storeId = 0)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (key == null)
                throw new ArgumentNullException("key");

            string keyGroup = entity.GetUnproxiedEntityType().Name;

            var props = GetAttributesForEntity(entity.Id, keyGroup).Where(m => m.StoreId == storeId).ToList();
            var prop = props.FirstOrDefault(m => m.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            var valueStr = CommonHelper.To<string>(value);
            if(prop != null)
            {
                if (string.IsNullOrEmpty(valueStr))
                {
                    DeleteAttribute(prop);
                }
                else
                {
                    UpdateAttribute(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(valueStr))
                {
                    prop = new GenericAttribute()
                    {
                        EntityId = entity.Id,
                        Key = key,
                        KeyGroup = keyGroup,
                        Value = valueStr,
                        StoreId = storeId
                    };
                    InsertAttribute(prop);
                }
            }
        }

        public void UpdateAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentException("attibute");

            _genericAttributeRepository.Update(attribute);

            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            _eventPublisher.EntityUpdated(attribute);
        }
    }
}
