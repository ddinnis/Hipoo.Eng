using FileService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.Commons;

namespace FileService.Domain
{
    public class FSDomainService
    {
        private readonly IFSRepository _repository;
        private readonly IStorageClient _backupStorage;
        private readonly IStorageClient _remoteStorage;

        public FSDomainService(IFSRepository repository, IEnumerable<IStorageClient> storageClients )
        {
            _backupStorage = storageClients.First(x=>x.StorageType == StorageType.Backup);
            _remoteStorage = storageClients.First(x => x.StorageType == StorageType.Public);
            _repository = repository;
        }

        public async Task<UploadedItem> UploadAsync(Stream stream, string fileName,
        CancellationToken cancellationToken) 
        {
            string hash = HashHelper.ComputeSha256Hash(stream);
            long fileSize = stream.Length;
            DateTime today = DateTime.Today;

            string key = $"{today.Year}/{today.Month}/{today.Day}/{hash}/{fileName}";

            var oldUploadItem =await _repository.FindFileAsync(fileSize,hash);

            if (oldUploadItem !=  null) { return oldUploadItem; }

            Uri backupUrl = await _backupStorage.SaveAsync(key,stream,cancellationToken);
            // 手动将流的位置重置
            stream.Position = 0;
            Uri remoteUrl = await _remoteStorage.SaveAsync(key, stream,cancellationToken);
            stream.Position = 0;

            Guid id = Guid.NewGuid();

            return UploadedItem.Create(id, fileSize, fileName, hash, backupUrl, remoteUrl);
        }
    }
}
