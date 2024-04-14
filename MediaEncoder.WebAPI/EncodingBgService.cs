
using MediaEncoder.Domain.Entities;
using MediaEncoder.Infrastructure;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using Common.Common;
using Zack.Commons;
using DotNetCore.CAP;
using MediaEncoder.Domain;
using Microsoft.Extensions.Options;
using MediaEncoder.WebAPI.Options;
using Common.JWT;
using FileService.SDK.NETCore;
using System.Net;

namespace MediaEncoder.WebAPI
{
    // 定时扫描未完成的转码的任务，然后进行转码
    public class EncodingBgService : BackgroundService
    {
        private readonly IServiceScope serviceScope;
        private readonly IMediaEncoderRepository repository;
        private readonly MediaEncoderDbContext dbContext;
        // RedLock
        private readonly List<RedLockMultiplexer> redLockMultiplexerList;
        private readonly ILogger<EncodingBgService> logger;

        private readonly ICapPublisher capPublisher;
        private readonly MediaEncoderFactory mediaEncoderFactory;
        private readonly IOptionsSnapshot<FileServiceOptions> optionFileService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptionsSnapshot<JWTOptions> optionJWT;
        private readonly ITokenService tokenService;

        public EncodingBgService(IServiceScopeFactory spf)
        {
            this.serviceScope = spf.CreateScope();
            var sp = serviceScope.ServiceProvider;
            IConnectionMultiplexer connectionMultiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            this.dbContext = sp.GetRequiredService<MediaEncoderDbContext>(); ;
            this.redLockMultiplexerList = new List<RedLockMultiplexer> { new RedLockMultiplexer(connectionMultiplexer) };
            this.logger = sp.GetRequiredService<ILogger<EncodingBgService>>();
            this.httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            this.mediaEncoderFactory = sp.GetRequiredService<MediaEncoderFactory>();
            this.optionFileService = sp.GetRequiredService<IOptionsSnapshot<FileServiceOptions>>();
            this.capPublisher = sp.GetRequiredService<ICapPublisher>();
            this.optionJWT = sp.GetRequiredService<IOptionsSnapshot<JWTOptions>>();
            this.tokenService = sp.GetRequiredService<ITokenService>();
            this.repository = sp.GetRequiredService<IMediaEncoderRepository>();
        }

        protected override async Task ExecuteAsync(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested) 
            {
                var readyItems = await repository.FindAsync(ItemStatus.Ready);
                foreach (var readyItem in readyItems)
                {
                    try
                    {
                        //因为转码比较消耗cpu等资源，因此串行转码
                        await ProcessItemAsync(readyItem, ct);
                    }
                    catch (Exception ex)
                    {
                        readyItem.Fail(ex);
                    }
                    await this.dbContext.SaveChangesAsync(ct);
                }
                //暂停5s，避免没有任务的时候CPU空转
                await Task.Delay(5000);
            }
        }

        private async Task ProcessItemAsync(EncodingItem encodingItem, CancellationToken ct)
        {
            Guid id = encodingItem.Id;
            var expiry = TimeSpan.FromSeconds(30);
            var redlockFactory = RedLockFactory.Create(redLockMultiplexerList);

            string lockKey = $"MediaEncoder.EncodingItem.{id}";
            await using (var redLock = await redlockFactory.CreateLockAsync(lockKey, expiry)) // there are also non async Create() methods
            {
                // make sure we got the lock
                // 如果没有获取到锁
                if (!redLock.IsAcquired)
                {
                    // do stuff
                    logger.LogWarning($"获取{lockKey}锁失败，已被抢走");
                    return;
                }

                encodingItem.Start();
                await dbContext.SaveChangesAsync();

                // 下载src
                (var isDownloadOk, var srcFile) = await DownloadSrcAsync(encodingItem,ct);
                if (!isDownloadOk) 
                {
                    encodingItem.Fail("下载失败");
                }
                FileInfo destFile = GetDestFileInfo(encodingItem);
                try
                {
                    logger.LogInformation($"下载Id={id}成功，开始计算Hash值");
                    long fileSize = srcFile.Length;
                    string srcFileHash = ComputeSha256Hash(srcFile);

                    // 是否找到已上传重复的数据
                    var prevInstance = await repository.FindCompletedOneAsync(srcFileHash, fileSize);
                    if (prevInstance != null)
                    {
                        logger.LogInformation($"检查Id={id}Hash值成功，发现已经存在相同大小和Hash值的旧任务Id={prevInstance.Id}，返回！");

                        capPublisher.Publish("MediaEncoding.Duplicated", new { encodingItem.Id, encodingItem.SourceSystem, OutputUrl = prevInstance.OutputUrl });

                        encodingItem.Complete(prevInstance.OutputUrl);
                        return;
                    }

                    //开始转码
                    logger.LogInformation($"Id={id}开始转码，源路径:{srcFile},目标路径:{destFile}");
                    string outputFormat = encodingItem.OutputFormat;
                    var encodingOK = await EncodeAsync(srcFile, destFile, outputFormat, ct); ;
                    if (!encodingOK)
                    {
                        encodingItem.Fail($"转码失败");
                        return;
                    }

                    //开始上传
                    logger.LogInformation($"Id={id}转码成功，开始准备上传");
                    Uri destUrl = await UploadFileAsync(destFile,ct);
                    encodingItem.Complete(destUrl);
                    encodingItem.ChangeFileMeta(fileSize, srcFileHash);
                    logger.LogInformation($"Id={id}转码结果上传成功");

                }
                finally 
                {
                    destFile.Delete();
                    srcFile.Delete();
                }
            }
        }

        private async Task<Uri> UploadFileAsync(FileInfo destFile,  CancellationToken ct)
        {
            Uri urlRoot = optionFileService.Value.UrlRoot;
            FileServiceClient fileService = new FileServiceClient(httpClientFactory,
                urlRoot, optionJWT.Value, tokenService);
            var url =await fileService.UploadAsync(destFile,ct);
            return url;
        }

        private async Task<bool> EncodeAsync(FileInfo srcFile, FileInfo destFile, string outputFormat, CancellationToken ct)
        {
            // 判断是否能转码
            var encoder = mediaEncoderFactory.Create(outputFormat);

            if (encoder == null)
            {
                logger.LogInformation($"转码失败，找不到转码器，目标格式:{outputFormat}");
                return false;
            }

            try
            {
               await encoder.EncodeAsync(srcFile,destFile, outputFormat,null,ct);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"转码失败", ex);
                return false;
            }
        }

        private static string ComputeSha256Hash(FileInfo file)
        {
            using (FileStream streamSrc = file.OpenRead())
            {
                return HashHelper.ComputeSha256Hash(streamSrc);
            }
        }

        private FileInfo GetDestFileInfo(EncodingItem encodingItem)
        {
            string outputFormat = encodingItem.OutputFormat;
            string tempDir = Path.GetTempPath();
            string destFullPath = Path.Combine(tempDir, Guid.NewGuid() + "." + outputFormat);
            var file =  new FileInfo(destFullPath);
            return file;
        }

        // 下载原视频
        private async Task<(bool isDownloadOk, FileInfo srcFile)> DownloadSrcAsync(EncodingItem encodingItem, CancellationToken ct)
        {
            //开始下载源文件
            string tempDir = Path.Combine(Path.GetTempPath(), "MediaEncodingDir");
            //源文件的临时保存路径
            string sourceFullPath = Path.Combine(tempDir, Guid.NewGuid() + "."
                + Path.GetExtension(encodingItem.Name));
            FileInfo sourceFile = new FileInfo(sourceFullPath);
            Guid id = encodingItem.Id;
            // 确保 sourceFile 所在的目录存在
            sourceFile.Directory!.Create();
            logger.LogInformation($"Id={id}，准备从{encodingItem.SourceUrl}下载到{sourceFullPath}");

            HttpClient httpClient = httpClientFactory.CreateClient();
            var statusCode = await httpClient.DownloadFileAsync(encodingItem.SourceUrl,sourceFullPath,ct);
            if (statusCode != HttpStatusCode.OK)
            {
                logger.LogInformation($"下载Id={id}，Url={encodingItem.SourceUrl}失败，{statusCode}");
                sourceFile.Delete();
                return (false, sourceFile);
            }
            else
            {
                return (true, sourceFile);
            }
        }
    }
}
