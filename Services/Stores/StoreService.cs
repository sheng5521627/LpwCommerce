using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Stores;
using Core.Data;
using Services.Events;
using Core.Caching;
using Core.Events;

namespace Services.Stores
{
    public partial class StoreService : IStoreService
    {
        #region Constants

        private const string STORES_ALL_KEY = "Nop.stores.all";
        private const string STORES_BY_ID_KEY = "Nop.stores.id-{0}";
        private const string STORES_PATTERN_KEY = "Nop.stores.";

        #endregion

        #region Field

        private readonly IRepository<Store> _storeRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        public StoreService(
            IRepository<Store> storeRepository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            _storeRepository = storeRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
        }


        public void DeleteStore(Store store)
        {
            if (store == null)
                throw new ArgumentException("store");

            var allStores = GetAllStores();
            if (allStores.Count == 1)
                throw new Exception("不能删除仅有的一个配置商店");
            _storeRepository.Delete(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            _eventPublisher.EntityDeleted(store);
        }

        public IList<Store> GetAllStores()
        {
            string key = STORES_ALL_KEY;
            return _cacheManager.Get(key, () => 
            {
                var query = from store in _storeRepository.Table
                            orderby store.DisplayOrder, store.Id
                            select store;
                return query.ToList();
            });
        }

        public Store GetStoreById(int storeId)
        {
            if (storeId == 0)
                return null;
            string key = string.Format(STORES_BY_ID_KEY, storeId);
            return _cacheManager.Get(key, () => _storeRepository.GetById(storeId));
        }

        public void InsertStore(Store store)
        {
            if (store == null)
                throw new ArgumentException("store");

            _storeRepository.Insert(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            _eventPublisher.EntityInserted(store);
        }

        public void UpdateStore(Store store)
        {
            if (store == null)
                throw new ArgumentException("store");

            _storeRepository.Insert(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            _eventPublisher.EntityUpdated(store);
        }
    }
}
