using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listening.Infrastructure.Configs
{
    public class EpisodeConfig : IEntityTypeConfiguration<Episode>
    {
        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            builder.ToTable("T_Episodes");
            builder.HasKey(e => e.Id).IsClustered(false);//Guid类型不要聚集索引，否则会影响性能
            builder.HasIndex(e => new { e.AlbumId, e.IsDeleted });//索引不要忘了加上IsDeleted，否则会影响性能
            builder.OwnsOneMultilingualString(e => e.Name);
            builder.Property(e => e.AudioUrl).HasMaxLength(1000).IsUnicode().IsRequired();
            builder.Property(e => e.Subtitle).HasMaxLength(int.MaxValue).IsUnicode().IsRequired();
            builder.Property(e => e.SubtitleType).HasMaxLength(10).IsUnicode(false).IsRequired();
        }
    }
}
