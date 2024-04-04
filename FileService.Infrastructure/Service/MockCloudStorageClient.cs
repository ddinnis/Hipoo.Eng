using FileService.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Service
{
    public class MockCloudStorageClient : IStorageClient
    {
        //提供对当前 HttpContext的访问（如果有）
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MockCloudStorageClient(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        // 供公众访问的存储设备
        public StorageType StorageType => StorageType.Public;

        public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            if (key.StartsWith("/")) 
            {
                throw new ArgumentException("key should not start with /", nameof(key));
            }
            string workingDir = Path.Combine(_webHostEnvironment.ContentRootPath,key);
            string fullPath = Path.GetDirectoryName(workingDir);

            if (!File.Exists(workingDir))
            {
                Directory.CreateDirectory(fullPath);
            }
            else 
            {
                File.Delete(fullPath);
            }
            using Stream stream = File.OpenWrite(fullPath);
            await stream.CopyToAsync(content);

            var req = _httpContextAccessor.HttpContext.Request;
            string url = req.Scheme + "://" + req.Host + "/FileService/" + key;
            return new Uri(url);
        }
    }
}
