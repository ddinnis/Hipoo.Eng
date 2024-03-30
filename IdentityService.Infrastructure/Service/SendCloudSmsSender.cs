using Common.Common;
using IdentityService.Domain;
using IdentityService.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityService.Infrastructure.Service
{
    public class SendCloudSmsSender: ISmsSender
    {
        private readonly ILogger<SendCloudSmsSender> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptionsSnapshot<SendCloudSmsSettings> _smsSettings;

        public SendCloudSmsSender(ILogger<SendCloudSmsSender> logger, IHttpClientFactory httpClientFactory, IOptionsSnapshot<SendCloudSmsSettings>  smsSettings)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _smsSettings = smsSettings;
        }

        public async Task SendAsync(string phoneNum, params string[] args)
        {
            _logger.LogInformation("Send Sms to {0},args:{1}",phoneNum,string.Join("|",args));
            var postBody = new Dictionary<string, string>();
            postBody["smsUser"] = _smsSettings.Value.SmsUser;
            postBody["templateId"] = "10010";
            postBody["phone"] = phoneNum;
            postBody["vars"] = args.ToJsonString();

            using (FormUrlEncodedContent httpContent = new FormUrlEncodedContent(postBody)) 
            {
                var httpClient = _httpClientFactory.CreateClient();
                var responseMsg = await httpClient.PostAsync("https://api.sendcloud.net/apiv2/mail/send", httpContent);
                if (!responseMsg.IsSuccessStatusCode)
                {
                    throw new Exception($"发送邮件响应码错误:{responseMsg.StatusCode}");
                }
                var respBody = await responseMsg.Content.ReadAsStringAsync();
                var respModel = respBody.ParseJson<SendCloudResponseModel>();
                if (!respModel.Result)
                {
                    throw new Exception($"发送邮件响应返回失败，状态码：{respModel.StatusCode},消息：{respModel.Message}");
                }
            }
        }
    }
}
