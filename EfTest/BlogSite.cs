using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfTest
{
    public class BlogSite
    {
        public int BogId { get; set; }
        public string BlogApp { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }

    public class BlogUser
    {
        public int UserId { get; set; }

        public string Author { get; set; }
    }
}
