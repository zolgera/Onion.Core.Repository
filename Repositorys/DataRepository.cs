using Core.Data.Entitys;
using Core.Repository.Data;
using Core.Repository.Interfaces;

namespace Core.Repository.Repositorys
{
    public class DataRepository<T> : EntityFrameworkRepository<T> where T : BaseAuditClass, new()
    {
        public DataRepository(DataContext context, IAudit audit) : base(context, audit)
        {
        }
    }
}
