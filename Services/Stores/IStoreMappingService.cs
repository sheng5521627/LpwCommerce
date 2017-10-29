using Core;
using Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Stores
{
    public partial interface IStoreMappingService
    {
        IList<StoreMapping> GetAllStoreMappings();

        void DeleteStoreMapping(StoreMapping storeMapping);

        StoreMapping GetStoreMappingById(int storeMappingId);

        IList<StoreMapping> GetStoreMappings<T>(T entity) where T : BaseEntity, IStoreMappingSupported;

        void InsertStoreMapping(StoreMapping storeMapping);

        void InsertStoreMapping<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported;

        void UpdateStoreMapping(StoreMapping storeMapping);

        int[] GetStoreIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported;
        
        bool Authorize<T>(T entity) where T : BaseEntity, IStoreMappingSupported;

        bool Authorize<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>Store identifiers</returns>
        int[] GetStoresIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported;

    }
}
