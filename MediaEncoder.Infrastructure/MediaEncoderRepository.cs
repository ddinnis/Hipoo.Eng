using MediaEncoder.Domain;
using MediaEncoder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaEncoder.Infrastructure
{
    public class MediaEncoderRepository : IMediaEncoderRepository
    {
        private readonly MediaEncoderDbContext mediaEncoderDbContext;

        public MediaEncoderRepository(MediaEncoderDbContext mediaEncoderDbContext)
        {
            this.mediaEncoderDbContext = mediaEncoderDbContext;
        }
        public async Task<EncodingItem[]> FindAsync(ItemStatus status)
        {
            var encItem = await mediaEncoderDbContext.EncodingItems.Where(e => e.Status == ItemStatus.Ready)
                .ToArrayAsync(); ;
            return encItem;
        }

        public async Task<EncodingItem?> FindCompletedOneAsync(string fileHash, long fileSize)
        {
            var encItems =  await mediaEncoderDbContext.EncodingItems.FirstOrDefaultAsync(e => e.FileSHA256Hash == fileHash
                      && e.FileSizeInBytes == fileSize && e.Status == ItemStatus.Completed);
            return encItems;
        }
    }
}
