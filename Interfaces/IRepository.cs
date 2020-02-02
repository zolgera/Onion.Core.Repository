using Core.Data.Entitys;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Repository.Interfaces
{
    public interface IRepository<T> where T: BaseAuditClass
    {
        IEnumerable<T> GetAll(IQueryOptions queryOptions = null);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter,IQueryOptions queryOptions = null);
        Task<IEnumerable<T>> GetAllAsync(IQueryOptions queryOptions = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter,IQueryOptions queryOptions = null, IEnumerable<string> includeProperties = null);
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> FindAsync<TKey>(Expression<Func<T, bool>> filter, Expression<Func<T, TKey>> orderBy, bool desc = false, int? top = null);
        bool Any(Expression<Func<T, bool>> filter);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        T Get(Guid id);
        Task<T> GetAsync(Guid id, IEnumerable<string> includeProperties = null);
        T GetByCode(string code);
        Task<T> GetByCodeAsync(string code);
        T Insert(T entity);
        Task<T> InsertAsync(T entity);
        T Update(T entity);
        Task<T> UpdateAsync(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        void Remove(T entity);
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
