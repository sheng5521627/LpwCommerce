using Core;
using Core.Domain.Stores;
using Core.Infrastructure;
using Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest.Stores
{
    public class StoreMappingTest
    {
        public void Test()
        {
            IStoreMappingService service = EngineContext.Current.Resolve<IStoreMappingService>();
            var list = service.GetAllStoreMappings();
        }
    }
    public class MyMapping : BaseEntity, IStoreMappingSupported
    {
        public bool LimitedToStores { get; set; }
    }
}
