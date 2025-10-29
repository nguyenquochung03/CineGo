using CineGo.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CineGo.Services.Implementations
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            // ✅ Demo giả lập gửi tin nhắn
            await Task.Run(() =>
            {
                _logger.LogInformation($"📱 [SMS GỬI ĐẾN] {phoneNumber}: {message}");
                Console.WriteLine($"📱 SMS đến {phoneNumber}: {message}");
            });

            // 🔧 Sau này có thể tích hợp API thật:
            // await _twilioClient.Messages.CreateAsync(
            //     to: new PhoneNumber(phoneNumber),
            //     from: new PhoneNumber(_config["Twilio:FromNumber"]),
            //     body: message);
        }
    }
}
