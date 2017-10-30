using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Framework.Events
{
    /// <summary>
    /// Product search event
    /// </summary>
    public class ProductSearchEvent
    {
        public string SearchTerm { get; set; }
        public bool SearchInDescriptions { get; set; }
        public IList<int> CategoryIds { get; set; }
        public int ManufacturerId { get; set; }
        public int WorkingLanguageId { get; set; }
        public int VendorId { get; set; }
    }
}
