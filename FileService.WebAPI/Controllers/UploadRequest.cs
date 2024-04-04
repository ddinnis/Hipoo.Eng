﻿using FluentValidation;

namespace FileService.WebAPI.Controllers
{
    public class UploadRequest
    {
        // IFormFile 用于处理或保存文件的表示形式
        public IFormFile File { get; set; }
    }
    public class UploadRequestValidator:AbstractValidator<UploadRequest> 
    {
        public UploadRequestValidator()
        {
            //不用校验文件名的后缀，因为文件服务器会做好安全设置，所以即使用户上传exe、php等文件都是可以的
            long maxFileSize = 50 * 1024 * 1024;//最大文件大小
            RuleFor(e => e.File).NotNull().Must(f => f.Length > 0 && f.Length < maxFileSize);
        }
    }
}
