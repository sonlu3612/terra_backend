using Microsoft.Extensions.Caching.Memory;

namespace Server.Services
{
    public class OtpService: IOtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(otp);
        }

        public void StoreOtp(string userId, string otp)
        {
            _cache.Set(userId, otp, TimeSpan.FromMinutes(5)); // Store OTP for 5 minutes
        }

        public async Task<bool> ValidateOtpAsync(string userId, string otp)
        {
            if (_cache.TryGetValue(userId, out string cachedOtp))
            {
                if (cachedOtp == otp)
                {
                    _cache.Remove(userId); // Invalidate OTP after successful validation
                    return true;
                }
            }
            return false;
        }
    }
}
