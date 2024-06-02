using System.Net;

namespace Commons.Common
{
    public static class HttpHelper
    {
        public static async Task<HttpStatusCode> DownloadFileAsync(this HttpClient httpClient,Uri uri, string localFile,CancellationToken cancellationToken = default) 
        {
            var responseMessage = await httpClient.GetAsync(uri,cancellationToken);
            if (responseMessage.IsSuccessStatusCode)
            {
                using FileStream fileStream = new FileStream(localFile, FileMode.Create);
                await responseMessage.Content.CopyToAsync(fileStream);
                return HttpStatusCode.OK;
            }
            else
            {
                return responseMessage.StatusCode;
            }
        }
    }
}
