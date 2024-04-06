using Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Reflection;



namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public static class MultilingualStringEFCoreExtensions
    {
        public static EntityTypeBuilder<TEntity> OwnsOneMultilingualString<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
            Expression<Func<TEntity, MultilingualString>> navigationExpression, bool required = true, int maxLength = 200) where TEntity : class
        {
            entityTypeBuilder.OwnsOne(navigationExpression, dp =>
            {
                dp.Property(c => c.Chinese).IsRequired(required).HasMaxLength(maxLength).IsUnicode();
                dp.Property(c => c.English).IsRequired(required).HasMaxLength(maxLength).IsUnicode();
            });
            // 用来设置导航属性的nullability（即是否允许为null）。
            entityTypeBuilder.Navigation(navigationExpression).IsRequired(required);
            return entityTypeBuilder;
        }
    }
}
