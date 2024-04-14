using FFmpeg.NET;
using MediaEncoder.Domain;
using Microsoft.Extensions.Logging;

namespace MediaEncoder.Infrastructure
{
    public class ToM4AEncoder : IMediaEncoder
    {
        private readonly ILogger<ToM4AEncoder> logger;
        public ToM4AEncoder(ILogger<ToM4AEncoder> logger)
        {
            this.logger = logger;
        }
        public bool Accept(string outputFormat)
        {
            var isAccept = "m4a".Equals(outputFormat, StringComparison.OrdinalIgnoreCase);
            return isAccept;
        }

        public async Task EncodeAsync(FileInfo sourceFile, FileInfo destFile, string destFormat, string[]? args, CancellationToken ct)
        {
            var inputFile = new InputFile(sourceFile);
            var outputFile = new OutputFile(destFile);

            string baseDir = AppContext.BaseDirectory;
            string ffmpegPath = Path.Combine(baseDir, "ffmpeg.exe");
            var ffmpeg = new Engine(ffmpegPath);

            string? errMSg = null;
            ffmpeg.Error += (s, e) => {
                errMSg = e.Exception.Message;
            };
            await ffmpeg.ConvertAsync(inputFile, outputFile, ct);
            if (errMSg != null)
            {
                logger.LogError("转码失败" + errMSg);
                throw new Exception(errMSg);
            }
        }
    }
}
