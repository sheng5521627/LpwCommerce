using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Stores;
using Core.Data;
using Core.Caching;
using Services.Events;
using Core.Domain.Catalog;

namespace Services.Stores
{
    public partial class StoreMappingService : IStoreMappingService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        private const string STOREMAPPING_BY_ENTITYID_NAME_KEY = "Nop.storemapping.entityid-name-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STOREMAPPING_PATTERN_KEY = "Nop.storemapping.";

        #endregion

        #region Fields

        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        public StoreMappingService(ICacheManager cacheManager,
            IStoreContext storeContext,
            IRepository<StoreMapping> storeMappingRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._storeMappingRepository = storeMappingRepository;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }

        public IList<StoreMapping> GetAllStoreMappings()
        {
            var query = from s in _storeMappingRepository.Table
                        orderby s.EntityName, s.StoreId
                        select s;
            return query.ToList();
        }

        public bool Authorize<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            return Authorize(entity, _storeContext.CurrentStore.Id);
        }

        public bool Authorize<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                return false;
            if (storeId == 0)
                return true;
            if (_catalogSettings.IgnoreStoreLimitations)
                return true;
            if (!entity.LimitedToStores)
                return true;

            foreach(var storeIdWithAccess in GetStoreIdsWithAccess(entity))
            {
                if (storeId == storeIdWithAccess)
                    return true;
            }
            return false;
        }

        public void DeleteStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentException("storeMapping");

            _storeMappingRepository.Delete(storeMapping);

            _cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);

            _eventPublisher.EntityDeleted(storeMapping);
        }

        public int[] GetStoreIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;
            string key = string.Format(STOREMAPPING_BY_ENTITYID_NAME_KEY, entityId, entityName);

            return _cacheManager.Get(key, () =>
            {
                var query = from s in _storeMappingRepository.Table
                            where s.EntityId == entityId && s.EntityName == entityName
                            select s.StoreId;
                return query.ToArray();
            });
        }

        public StoreMapping GetStoreMappingById(int storeMappingId)
        {
            if (storeMappingId == 0)
                return null;

            return _storeMappingRepository.GetById(storeMappingId);
        }

        public IList<StoreMapping> GetStoreMappings<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var query = from s in _storeMappingRepository.Table
                        where s.EntityId == entityId && s.EntityName == entityName
                        select s;
            return query.ToList();
        }

        public void InsertStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new Exception("storeMapping");

            _storeMappingRepository.Insert(storeMapping);

            _cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);

            _eventPublisher.EntityInserted(storeMapping);
        }

        public void InsertStoreMapping<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new Exception("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;
            var storeMapping = new StoreMapping()
            {
                EntityId = entityId,
                EntityName = entityName,
                StoreId = storeId
            };

            InsertStoreMapping(storeMapping);
        }

        public void UpdateStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentException("storeMapping");

            _storeMappingRepository.Update(storeMapping);

            _cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);

            _eventPublisher.EntityUpdated(storeMapping);
        }
    }
}
