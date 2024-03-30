using IdentityService.Domain;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class MockEmailSender: IEmailSender
    {
        private readonly ILogger<MockEmailSender> _logger;

        public Task SendAsync(string toEmail, string subject, string body)
        {
            _logger.LogInformation("Send Email to {0},title:{1}, body:{2}", toEmail, subject, body);
            return Task.CompletedTask;
        }
    }
}
