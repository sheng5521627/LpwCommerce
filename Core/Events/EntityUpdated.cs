using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Events
{
    public class EntityUpdated<T> where T : BaseEntity
    {
        public T Entity { get; private set; }

        public EntityUpdated(T entity)
        {
            this.Entity = entity;
        }
    }
}
