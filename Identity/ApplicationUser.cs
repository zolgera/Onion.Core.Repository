using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repository.Identity
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string Storage { get; set; }
        public Guid? IdAccount { get; set; }
        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(string userName, string email) : base(userName, email)
        {
        }
    }
}
