using Core;
using Core.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields

        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        #endregion

        public EfRepository(IDbContext context)
        {
            _context = context;
        }

        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
        }

        public virtual IQueryable<T> Table
        {
            get { return this.Entities; }
        }

        public virtual IQueryable<T> TableNoTracking
        {
            get { return this.Entities.AsNoTracking(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        protected string GetFullErrorText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach(var validationError in exc.EntityValidationErrors)
            {
                foreach(var error in validationError.ValidationErrors)
                {
                    msg += string.Format("Property: {0} Error: {1}", error.PropertyName, error.ErrorMessage) + Environment.NewLine; 
                }
            }
            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentException("entity");
                this.Entities.Add(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {
                throw new NopException(GetFullErrorText(exc), exc);
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentException("entities");

                foreach (var entity in entities)
                    this.Entities.Add(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {
                throw new NopException(GetFullErrorText(exc), exc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentException("entity");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {
                throw new NopException(GetFullErrorText(exc), exc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Update(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentException("entities");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {
                throw new NopException(GetFullErrorText(exc), exc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentException("entity");

                this.Entities.Remove(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {
                throw new NopException(GetFullErrorText(exc), exc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    this.Entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {
                throw new Exception(GetFullErrorText(exc), exc);
            }
        }
    }
}
