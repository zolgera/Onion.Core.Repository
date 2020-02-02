using Core.Data.Interfaces.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Repository.Maps
{
    public class EntityMap
    {
        private EntityMap()
        {

        }

        public static EntityTypeBuilder<TEntity> CreateBaseEntity<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : class, IBaseEntity
        {
            entityBuilder.HasKey(o => o.Id);
            entityBuilder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            return entityBuilder;
        }
        public static EntityTypeBuilder<TEntity> CreateBaseDocument<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, string shortCode = null) where TEntity : class, IBaseDocument
        {
            entityBuilder = CreateBaseEntity(entityBuilder);
            entityBuilder.HasIndex(e => e.Code).IsUnique();
            return entityBuilder;
        }
    }
}
