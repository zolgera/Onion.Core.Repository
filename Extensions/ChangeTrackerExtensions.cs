using Core.Data.Entitys;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace Core.Repository.Extensions
{
    public static class ChangeTrackerExtensions
    {
        public static void ApplyAuditInformation(this ChangeTracker changeTracker)
        {
            foreach (var entry in changeTracker.Entries())
            {
                if (!(entry.Entity is BaseAuditClass baseAudit))
                    continue;
                var now = DateTime.UtcNow;
                switch (entry.State)
                {
                    case Microsoft.EntityFrameworkCore.EntityState.Modified:
                        baseAudit.Modified = now;
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Added:
                        baseAudit.Created = now;
                        baseAudit.Modified = now;
                        break;
                }
            }
        }
    }
}
