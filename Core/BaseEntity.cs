﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    public abstract partial class BaseEntity
    {
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(BaseEntity other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) && !IsTransient(other) && Equals(this.Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = this.GetUnproxiedType();

                return otherType.IsAssignableFrom(thisType) || thisType.IsAssignableFrom(otherType);
            }

            return false;
        }

        /// <summary>
        /// 判断实体是否是瞬态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static bool IsTransient(BaseEntity entity)
        {
            return entity != null && Equals(entity.Id, default(int));
        }

        /// <summary>
        /// 代理类
        /// </summary>
        /// <returns></returns>
        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public override int GetHashCode()
        {
            if(Equals(Id,default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity x,BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x,BaseEntity y)
        {
            return !(x == y);
        }
    }
}
