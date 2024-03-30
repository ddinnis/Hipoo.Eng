using IdentityService.Domain;
using IdentityService.Infrastructure.Options;
using IdentityService.Infrastructure.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Common.Common;


public class SendCloudEmailSender : IEmailSender
{
    private readonly ILogger<SendCloudEmailSender> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsSnapshot<SendCloudEmailSettings> _sendCloudSettings;
    public SendCloudEmailSender(ILogger<SendCloudEmailSender> logger,
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<SendCloudEmailSettings> sendCloudSettings)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _sendCloudSettings = sendCloudSettings;
    }

    public async Task SendAsync(string toEmail, string subject, string body)
    {
        _logger.LogInformation("SendCloud Email to {0},subject:{1},body:{2}", toEmail, subject, body);
        var postBody = new Dictionary<string, string>();
        postBody["apiUser"] = _sendCloudSettings.Value.ApiUser;
        postBody["apiKey"] = _sendCloudSettings.Value.ApiKey;
        postBody["from"] = _sendCloudSettings.Value.From;
        postBody["to"] = toEmail;
        postBody["subject"] = subject;
        postBody["html"] = body;

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