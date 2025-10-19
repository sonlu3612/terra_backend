using Auth.Models;

namespace Auth.Services
{
    public interface ITokenService
    {
        public string CreateToken(ApplicationUser user);
    }
}
