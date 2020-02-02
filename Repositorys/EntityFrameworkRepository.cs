using core.Interfaces.Query;
using Core.Data.Entitys;
using Core.Data.Interfaces.Entitys;
using Core.Data.Query;
using Core.Repository.Extensions;
using Core.Repository.Interfaces;
using Core.Repository.Query;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Core.Repository.Repositorys
{
    public abstract class EntityFrameworkRepository<T> : IRepository<T> where T : BaseAuditClass, new()
    {
        protected readonly DbContext context;
        protected DbSet<T> entities;
        string errorMessage = string.Empty;
        protected IAudit audit;

        public EntityFrameworkRepository(DbContext context, IAudit audit)
        {
            this.context = context;
            entities = context.Set<T>();
            this.audit = audit;
        }
        public bool Any(Expression<Func<T, bool>> filter)
        {
            return entities.AsQueryable().Any(filter);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return (await entities.AsQueryable().AnyAsync(filter));
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");
            entities.Remove(entity);
            context.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");
            entities.Remove(entity);
            await context.SaveChangesAsync();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return entities.AsQueryable().Where(filter).AsEnumerable();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return (await entities.AsQueryable().AsNoTracking().Where(filter).ToListAsync()).AsEnumerable();
        }
        public async Task<IEnumerable<T>> FindAsync<TKey>(Expression<Func<T, bool>> filter, Expression<Func<T, TKey>> orderBy, bool desc = false, int? top = null)
        {
            IQueryable<T> query = entities.AsQueryable().AsNoTracking().Where(filter);
            query = desc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            if (top != null)
            {
                query = query.Take(top ?? 0);
            }
            return (await query.ToListAsync()).AsEnumerable();
        }

        public T Get(Guid id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public IEnumerable<T> GetAll(IQueryOptions queryOptions = null)
        {
            IQueryable<T> query = entities.AsQueryable();
            return queryOptions != null ? query.QueryOptions(queryOptions) : entities.AsEnumerable();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, IQueryOptions queryOptions = null)
        {
            IQueryable<T> query = entities.AsQueryable();
            query = query.Where(filter);
            return queryOptions != null ? query.QueryOptions(queryOptions) : entities.AsEnumerable();
        }

        public async Task<IQueryResult<T>> GetAllAsync(ODataQueryOptions<T> queryOptions = null, IEnumerable<string> includeProperties = null)
        {
            IQueryable<T> query = entities.AsQueryable();
            if (includeProperties != null)
            {
                foreach (string include in includeProperties)
                {
                    query = query.Include(include);
                }
            }
            query = queryOptions.ApplyTo(query) as IQueryable<T>;
            PageInfo pageInfo = null;
            if (queryOptions != null)
            {
                long total = await entities.AsQueryable().LongCountAsync();
                if (queryOptions.Top?.Value > 0)
                {
                    pageInfo = new PageInfo(total, queryOptions.Skip?.Value, queryOptions.Top?.Value);
                }
            }
            IEnumerable<T> data = (await query.ToListAsync())?.AsEnumerable();

            return new QueryResult<T>(data, pageInfo);
        }

        public async Task<IEnumerable<T>> GetAllAsync(IQueryOptions queryOptions = null)
        {
            IQueryable<T> query = entities.AsQueryable();
            return queryOptions != null ? (await query.QueryOptionsAsQueryable(queryOptions).ToListAsync())?.AsEnumerable() : (await entities.AsQueryable().ToListAsync()).AsEnumerable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter, IQueryOptions queryOptions = null, IEnumerable<string> includeProperties = null)
        {
            IQueryable<T> query = entities.AsQueryable();
            if (includeProperties != null)
            {
                foreach (string include in includeProperties)
                {
                    query = query.Include(include);
                }
            }
            query = query.Where(filter);
            return queryOptions != null ? (await query.QueryOptionsAsQueryable(queryOptions).ToListAsync())?.AsEnumerable() : (await entities.AsQueryable().ToListAsync())?.AsEnumerable();
        }

        public async Task<IQueryResult<T>> GetAllAsync(Expression<Func<T, bool>> filter, ODataQueryOptions<T> queryOptions = null, IEnumerable<string> includeProperties = null)
        {
            IQueryable<T> query = entities.AsQueryable();
            if (includeProperties != null)
            {
                foreach (string include in includeProperties)
                {
                    query = query.Include(include);
                }
            }
            query = query.Where(filter);
            query = queryOptions.ApplyTo(query) as IQueryable<T>;
            PageInfo pageInfo = null;
            if (queryOptions != null)
            {
                long total = await entities.AsQueryable().LongCountAsync();
                if (queryOptions.Top?.Value > 0)
                {
                    pageInfo = new PageInfo(total, queryOptions?.Skip?.Value, queryOptions?.Top?.Value);
                }
            }
            IEnumerable<T> data = (await query.ToListAsync()).AsEnumerable();

            return new QueryResult<T>(data, pageInfo);
        }

        public async Task<T> GetAsync(Guid id, IEnumerable<string> includeProperties = null)
        {
            IQueryable<T> query = entities.AsQueryable();
            if (includeProperties != null)
            {
                foreach (string include in includeProperties)
                {
                    query = query.Include(include);
                }
            }
            return await query.SingleOrDefaultAsync(s => s.Id == id);
        }

        public T GetByCode(string code)
        {
            T ret = null;
            if (typeof(T).IsSubclassOf(typeof(BaseDocument)))
            {
                ret = entities.SingleOrDefault(s => ((IBaseDocument)s).Code == code);
            }
            return ret;
        }

        public async Task<T> GetByCodeAsync(string code)
        {
            T ret = null;
            if (typeof(T).IsSubclassOf(typeof(BaseDocument)))
            {
                ret = await entities.SingleOrDefaultAsync(s => ((IBaseDocument)s).Code == code);
            }
            return ret;
        }

        public T Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");

            if (entity.Id == null || entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            entity = audit.StampCreated(entity);

            entities.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public async Task<T> InsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");

            entity = audit.StampCreated(entity);

            if (entity.Id == default(Guid))
            {
                entities.Attach(entity);
            }
            else
            {
                entities.Add(entity);
            }

            await context.SaveChangesAsync();
            return entity;
        }

        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");
            entities.Remove(entity);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public T Update(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");
            entity = audit.StampModifed(entity);
            context.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
            return entity;
        }
        internal void Upsert(T entity)
        {
            context.ChangeTracker.TrackGraph(entity, e =>
            {
                if (e.Entry.IsKeySet)
                {
                    e.Entry.State = EntityState.Modified;
                }
                else
                {
                    e.Entry.State = EntityState.Added;
                }
            });

#if DEBUG
            foreach (var entry in context.ChangeTracker.Entries())
            {
                Debug.WriteLine($"Entity: {entry.Entity.GetType().Name} State: {entry.State.ToString()}");
            }
#endif
        }
        public async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");

            entity = audit.StampModifed(entity);
            if (context.Entry(entity).State == EntityState.Detached)
            {
                context.Attach(entity);
            }
            context.Entry(entity).State = EntityState.Modified;
            //Upsert(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public T Create()
        {
            T ret = new T
            {
                Id = Guid.NewGuid()
            };
            return ret;
        }

        public async Task<int> GetNextSequenceValue(string sequenceName)
        {
            SqlParameter result = new SqlParameter("@result", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            SqlParameter name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 25);
            name.Value = sequenceName;
            string query = $"SELECT @result = (NEXT VALUE FOR {sequenceName})";
            await context.Database.ExecuteSqlRawAsync(query, new SqlParameter[] { result, name });
            return (int)result.Value;
        }
    }
}
