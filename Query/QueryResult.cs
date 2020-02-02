using core.Interfaces.Query;
using Core.Data.Interfaces.Entitys;
using Core.Data.Query;
using System.Collections.Generic;

namespace Core.Repository.Query
{
    public class QueryResult<T> : IQueryResult<T> where T : IBaseEntity
    {
        public QueryResult()
        {

        }
        public QueryResult(IEnumerable<T> payload, PageInfo pageInfo)
        {
            Payload = payload;
            PageInfo = pageInfo;
        }
        public QueryResult(IEnumerable<T> payload)
        {
            Payload = payload;
        }
        public IEnumerable<T> Payload { get; private set; }
        public PageInfo PageInfo { get; private set; }
    }
}
