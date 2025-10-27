namespace Server.Services
{
    public class SmsService:ISmsService
    {
        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            // Implementation for sending SMS
            Console.WriteLine($"Sending SMS to {phoneNumber}: {message}\n");
            await Task.CompletedTask;
        }

    }
}
