namespace Server.Services
{
    public interface IOtpService
    {
        string GenerateOtp(int length = 6);
        void StoreOtp(string userId, string otp);
        Task<bool> ValidateOtpAsync(string userId, string otp);
    }
}
