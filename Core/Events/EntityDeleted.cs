using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Events
{
    public class EntityDeleted<T> where T : BaseEntity
    {
        public T Entity { get; private set; }

        public EntityDeleted(T entity)
        {
            this.Entity = entity;
        }
    }
}
