using Core.Repository.Data;
using Core.Repository.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Repository.Extensions;
using Microsoft.AspNet.OData.Query;
using core.Interfaces.Query;
using Core.Repository.Query;
using Core.Data.Entitys;
using Core.Data.Interfaces.Audit;
using Core.Data.Query;

namespace Core.Repository.Repositorys
{
    public class MongoRepository<T> : IRepository<T> where T : BaseDocument, IAuditable
    {
        private IMongoCollection<T> entities;
        private readonly MongoDataContext context;
        string errorMessage = string.Empty;
        IAudit audit;

        public MongoRepository(IAudit audit)
        {
            this.audit = audit;
            Type classType = typeof(T);
            context = new MongoDataContext();
            entities = context.MongoDatabase.GetCollection<T>(classType.Name);
        }

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return entities.AsQueryable().Any(filter);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await entities.AsQueryable().AnyAsync(filter);
        }

        public void Delete(T entity)
        {
            entities.DeleteOne(e => e.Id == entity.Id);
        }

        public async Task DeleteAsync(T entity)
        {
            await entities.DeleteOneAsync(e => e.Id == entity.Id);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return entities.AsQueryable().Where(filter).AsEnumerable();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await entities.AsQueryable().Where(filter).ToListAsync();
        }
        public async Task<IEnumerable<T>> FindAsync<TKey>(Expression<Func<T, bool>> filter, Expression<Func<T, TKey>> orderBy, bool desc = false, int? top = null)
        {
            IMongoQueryable<T> query = entities.AsQueryable().Where(filter);
            query = desc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            if (top != null)
            {
                query = query.Take(top ?? 0);
            }
            return await query.ToListAsync();
        }

        public IEnumerable<T> GetAll(IQueryOptions queryOptions = null)
        {
            IMongoQueryable<T> query = entities.AsQueryable();
            return queryOptions != null ? query.MongoQueryOptionsAsQueryable(queryOptions).ToList() : entities.AsQueryable().ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(IQueryOptions queryOptions = null)
        {
            IMongoQueryable<T> query = entities.AsQueryable();
            return queryOptions != null ? await query.MongoQueryOptionsAsQueryable(queryOptions).ToListAsync() : await entities.AsQueryable().ToListAsync();
        }

        public T Get(Guid id)
        {
            return entities.AsQueryable().SingleOrDefault(s => s.Id == id);
        }

        public async Task<T> GetAsync(Guid id, IEnumerable<string> includeProperties = null)
        {
            return await entities.Find(f=>f.Id == id).SingleOrDefaultAsync();
        }

        public T GetByCode(string code)
        {
            return entities.AsQueryable().SingleOrDefault(s => s.Name == code);
        }

        public async Task<T> GetByCodeAsync(string code)
        {
            return await entities.AsQueryable().SingleOrDefaultAsync(s => s.Name == code);
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

            entities.InsertOne(entity);
            return entity;
        }

        public async Task<T> InsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentException("entity");

            if (entity.Id == null || entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            entity = audit.StampCreated(entity);

            await entities.InsertOneAsync(entity);
            return entity;
        }

        public void Remove(T entity)
        {
            entities.DeleteOne(e => e.Id == entity.Id);
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public T Update(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentException("No data provided", nameof(entity));
            }
            if (entity.Id == null || entity.Id == Guid.Empty)
            {
                throw new ArgumentException("Provied model has no ID", nameof(entity));
            }
            entity = audit.StampModifed(entity);
            entities.ReplaceOne(e => e.Id == entity.Id, entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            entity = audit.StampModifed(entity);
            await entities.ReplaceOneAsync(e => e.Id == entity.Id, entity);
            return entity;
        }

        public async Task<IQueryResult<T>> GetAllAsync(ODataQueryOptions<T> queryOptions = null, IEnumerable<string> includeProperties = null)
        {
            // aaa query.MongoQueryOptionsAsQueryable(queryOptions)
            IMongoQueryable<T> query = queryOptions == null? entities.AsQueryable() : queryOptions.ApplyTo(entities.AsQueryable()) as IMongoQueryable<T>;
            PageInfo pageInfo = null;
            if(queryOptions != null)
            {
                long total = await entities.AsQueryable().LongCountAsync();
                int skip = queryOptions.Skip == null ? 0 : queryOptions.Skip.Value;
                if (queryOptions.Top?.Value > 0){
                    pageInfo = new PageInfo(total, skip, queryOptions.Top.Value);
                }
            }
            IEnumerable<T> data = await query.ToListAsync();

            return new QueryResult<T>(data, pageInfo);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, IQueryOptions queryOptions = null)
        {
            IMongoQueryable<T> query = entities.AsQueryable();
            query = query.Where(filter);
            return queryOptions != null ? query.QueryOptions(queryOptions) : entities.AsQueryable().AsEnumerable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter, IQueryOptions queryOptions = null, IEnumerable<string> includeProperties = null)
        {
            IMongoQueryable<T> query = entities.AsQueryable();
            query = query.Where(filter);
            return queryOptions != null ? await query.MongoQueryOptionsAsQueryable(queryOptions).ToListAsync() : await entities.AsQueryable().ToListAsync();
        }

        public async Task<IQueryResult<T>> GetAllAsync(Expression<Func<T, bool>> filter, ODataQueryOptions<T> queryOptions = null, IEnumerable<string> includeProperties = null)
        {
            IMongoQueryable<T> query = (queryOptions == null? entities.AsQueryable(): queryOptions.ApplyTo(entities.AsQueryable())) as IMongoQueryable<T>;
            query = query.Where(filter);
            PageInfo pageInfo = null;
            if (queryOptions != null)
            {
                long total = await entities.AsQueryable().LongCountAsync();
                int skip = queryOptions.Skip == null ? 0 : queryOptions.Skip.Value;
                if (queryOptions.Top?.Value > 0)
                {
                    pageInfo = new PageInfo(total, skip, queryOptions.Top.Value);
                }
            }
            IEnumerable<T> data = await query.ToListAsync();

            return new QueryResult<T>(data, pageInfo);
        }
    }
}
