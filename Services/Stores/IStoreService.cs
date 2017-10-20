using Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Stores
{
    public partial interface IStoreService
    {
        void DeleteStore(Store store);

        IList<Store> GetAllStores();

        Store GetStoreById(int storeId);

        void InsertStore(Store store);

        void UpdateStore(Store store);
    }
}
