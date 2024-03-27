using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        public static void EnableSoftDeletionGlobalFilter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
                        Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);
                    //一个全局查询过滤器，这个过滤器会自动排除那些IsDeleted属性为true的记录。
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }

}
