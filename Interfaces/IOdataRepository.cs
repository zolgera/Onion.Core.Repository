using core.Interfaces.Query;
using Core.Data.Entitys;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Repository.Interfaces
{
    public interface IOdataRepository<T> : IRepository<T> where T : BaseAuditClass
    {
        Task<IQueryResult<T>> GetAllAsync(ODataQueryOptions<T> queryOptions = null, IEnumerable<string> includeProperties = null);
        Task<IQueryResult<T>> GetAllAsync(Expression<Func<T, bool>> filter, ODataQueryOptions<T> queryOptions = null, IEnumerable<string> includeProperties = null);
    }
}
