using Core.Data.Interfaces.Entitys;
using Core.Data.Interfaces.Query;

namespace Core.Repository.Query
{
    public class DocumentResult<T> : IDocumentResult<T> where T : IBaseEntity
    {
        public DocumentResult(T payload)
        {
            Payload = payload;
        }
        public T Payload { get; private set; }
    }
}
