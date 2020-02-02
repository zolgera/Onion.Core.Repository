using Core.Repository.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Core.Data.Entitys.Security;

namespace Core.Repository.Data
{
    public class AuthContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }
        public DbSet<Token> Tokens { set; get; }
    }
}
