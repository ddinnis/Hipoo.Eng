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
            string? fullDir = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(fullDir))
            {
                Directory.CreateDirectory(fullDir);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            using Stream stream = File.OpenWrite(fullPath);
            await content.CopyToAsync(stream, cancellationToken);
            return new Uri(fullPath);
        }
    }
}
