using Core.Repository.Data;
using Core.Repository.Interfaces;
using Core.Data.Entitys;

namespace Core.Repository.Repositorys
{

    public class AuthDataRepository<T> : EntityFrameworkRepository<T> where T : BaseAuditClass, new()
    {
        public AuthDataRepository(AuthContext context, IAudit audit) : base(context, audit)
        {
        }
    }
}
