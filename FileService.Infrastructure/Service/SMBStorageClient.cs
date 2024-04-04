using FileService.Domain;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.Service
{
    public class SMBStorageClient : IStorageClient
    {
        private IOptionsSnapshot<SMBStorageOptions> options;
        public SMBStorageClient(IOptionsSnapshot<SMBStorageOptions> options)
        {
            this.options = options;
        }
        public StorageType StorageType => StorageType.Backup;

        public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            if (key.StartsWith("/")) 
            {
                throw new ArgumentException("key should not start with /", nameof(key));
            }
            string workingDir = options.Value.WorkingDir;
            string fullPath = Path.Combine(workingDir, key);
            string fullDir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(fullPath)) 
            {
                Directory.CreateDirectory(fullPath);
            }
            if (Directory.Exists(fullPath)) 
            { 
                File.Delete(fullPath); // 删除旧文件
            }
            using Stream stream = File.OpenWrite(fullPath);
            await content.CopyToAsync(stream);
            return new Uri("file://" + fullPath);
        }
    }
}
