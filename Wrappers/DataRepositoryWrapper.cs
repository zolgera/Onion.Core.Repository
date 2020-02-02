using Core.Repository.Data;
using Core.Repository.Interfaces;
using Core.Repository.Repositorys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository.Wrappers
{
    public class DataRepositoryWrapper : IDataRepositoryWrapper
    {
        private readonly DataContext context;
        private readonly IAudit audit;
        public DataRepositoryWrapper(DataContext context, IAudit audit)
        {
            this.context = context;
            this.audit = audit;
        }
        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
