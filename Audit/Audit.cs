using Core.Data.Interfaces.Audit;
using Core.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace Core.Repository.Audit
{
    public class Audit : IAudit
    {
        private readonly IHttpContextAccessor accessor;
        public Audit(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }
        public T StampCreated<T>(T model) where T : IAuditable
        {
            model.Created = DateTime.UtcNow;
            model.CreatedBy = accessor.HttpContext.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            return model;
        }
        public T StampModifed<T>(T model) where T : IAuditable
        {
            model.Modified = DateTime.UtcNow;
            model.ModifiedBy = accessor.HttpContext.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            return model;
        }
    }
}
