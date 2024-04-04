using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain
{
    public interface IStorageClient
    {
        StorageType StorageType { get; }
        Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default);

    }
}
