using FileService.Domain.Entities;
using FileService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain
{
    public class FSRepository : IFSRepository
    {
        private readonly FSDbContext _dbContext;

        public FSRepository(FSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UploadedItem?> FindFileAsync(long fileSize, string sha256Hash)
        {
            return _dbContext.UploadedItems.FirstOrDefaultAsync(u => u.FileSizeInBytes == fileSize
            && u.FileSHA256Hash == sha256Hash);
        }
    }
}
