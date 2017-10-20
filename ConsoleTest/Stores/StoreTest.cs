using Core.Infrastructure;
using Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest.Stores
{
    public class StoreTest
    {
        public void F()
        {
            IStoreService storeService = EngineContext.Current.Resolve<IStoreService>();
            var list = storeService.GetAllStores();
        }
    }
}
