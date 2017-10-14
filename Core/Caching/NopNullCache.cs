using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Caching
{
    public partial class NopNullCache : ICacheManager
    {
        public virtual void Clear()
        {
            
        }

        public virtual void Dispose()
        {
            
        }

        public virtual T Get<T>(string key)
        {
            return default(T);   
        }

        public virtual bool IsSet(string key)
        {
            return false;
        }

        public virtual void Remove(string key)
        {
            
        }

        public virtual void RemoveByPattern(string pattern)
        {
             
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
             
        }
    }
}
