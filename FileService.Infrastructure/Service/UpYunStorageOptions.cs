using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Service
{
    public class UpYunStorageOptions
    {
        public string BucketName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 上传的根目录
        /// </summary>
        public string WorkingDir { get; set; }

        /// <summary>
        /// http(s)://等这样开头的Url的根目录
        /// </summary>
        public string UrlRoot { get; set; }
    }
}
