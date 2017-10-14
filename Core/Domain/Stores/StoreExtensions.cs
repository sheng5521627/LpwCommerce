using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Stores
{
    public static class StoreExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static string[] ParseHostValues(this Store store)
        {
            if (store == null)
                throw new ArgumentException("store");

            var parsedValues = new List<string>();
            if (!string.IsNullOrEmpty(store.Hosts))
            {
                string[] hosts = store.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var item in hosts)
                {
                    var temp = item.Trim();
                    if (!string.IsNullOrEmpty(temp))
                        parsedValues.Add(temp);
                }
            }
            return parsedValues.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static bool ContainsHostValue(this Store store,string host)
        {
            if (store == null)
                throw new ArgumentException("store");

            if (String.IsNullOrEmpty(host))
                return false;

            var contains = store.ParseHostValues().Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));
            return contains;
        }
    }
}
