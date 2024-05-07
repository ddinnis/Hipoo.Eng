using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FileService.Infrastructure.Configs
{
    class UploadedItemConfig : IEntityTypeConfiguration<UploadedItem>
    {
        public void Configure(EntityTypeBuilder<UploadedItem> builder)
        {
            builder.ToTable("T_FS_UploadedItems");
            builder.HasKey(x => x.Id).IsClustered(false);
            builder.Property(e => e.FileName).IsUnicode().HasMaxLength(1024);
            builder.Property(e => e.FileSHA256Hash).IsUnicode(false).HasMaxLength(64);
            // 复合索引 提高查询效率
            builder.HasIndex(x => new { x.FileSHA256Hash, x.FileSizeInBytes });
        }
    }
}
