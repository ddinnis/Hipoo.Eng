using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace IdentityService.WebAPI.Controller
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController
    {
        private readonly ISmsSender _SendCloudSmsSender;
        public TestController(ISmsSender SendCloudSmsSender)
        {
            _SendCloudSmsSender = SendCloudSmsSender;
        }
        [HttpGet]
        public ActionResult<string> Get() {
            
            var result =  _SendCloudSmsSender.SendAsync("123456789", "Hello, World!","This is Tupi");
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            string jsonString = JsonSerializer.Serialize("雅思真题", options);

            string jsonStringtest = JsonSerializer.Serialize("雅思真题");



            return "Test";
        }
    }
}
