using IdentityService.Domain;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class MockSmsSender : ISmsSender
    {
        private readonly ILogger<MockSmsSender> _logger;
        public MockSmsSender(ILogger<MockSmsSender> logger)
        {
            _logger = logger;
        }
        public Task SendAsync(string phoneNum, params string[] args)
        {
            _logger.LogInformation("Send Sms to {0},args:{1}", phoneNum,
                string.Join(",", args));
            return Task.CompletedTask;
        }
    }
}
