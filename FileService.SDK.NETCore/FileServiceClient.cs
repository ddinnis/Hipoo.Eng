using Common.Common;
using Common.JWT;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace FileService.SDK.NETCore
{
    // 方便外界调用
    public class FileServiceClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly Uri serverRoot;
        private readonly JWTOptions optionsSnapshot;
        private readonly ITokenService tokenService;

        public FileServiceClient(IHttpClientFactory httpClientFactory, Uri serverRoot, JWTOptions optionsSnapshot, ITokenService tokenService)
        {
            this.httpClientFactory = httpClientFactory;
            this.serverRoot = serverRoot;
            this.optionsSnapshot = optionsSnapshot;
            this.tokenService = tokenService;
        }

        public async Task<FileExistsResponse> FileExistsAsync(long fileSize, string sha256Hash, CancellationToken stoppingToken = default) {
            string url = FormattableStringHelper.BuildUrl($"api/Upload/FileExists?fileSize={fileSize}&sha256Hash={sha256Hash}");
            Uri requestUri = new Uri(serverRoot, url);
            var httpClient = httpClientFactory.CreateClient();
            var strResult = await httpClient.GetStringAsync(requestUri, stoppingToken);
            var result = strResult.ParseJson<FileExistsResponse>();

            return result;
        }

        public async Task<Uri> UploadAsync(FileInfo file, CancellationToken stoppingToken = default) 
        {
            Claim claim = new Claim(ClaimTypes.Role, "Admin");
            string token = tokenService.BuildToken(new Claim[] { claim }, optionsSnapshot);
            
            // 上传文件或传递表单数据
            using var multipartContent = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenRead());
            multipartContent.Add(fileContent, "file", file.Name);
            var httpClient = httpClientFactory.CreateClient();

            string serverRoot = "http://localhost:81";
            Uri requestUri = new Uri(serverRoot + "/FileService/Upload/UploadFile");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var responseMessage = await httpClient.PostAsync(requestUri,multipartContent,stoppingToken);

            string respString = await responseMessage.Content.ReadAsStringAsync(stoppingToken);

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"上传失败，状态码：{responseMessage.StatusCode}，响应报文：\n{respString}");
            }
            else
            {
                return respString.ParseJson<Uri>()!;
            }
        }

    }
}
