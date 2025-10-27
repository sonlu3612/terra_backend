using Server.Models;

namespace Server.Services
{
    public interface ITokenService
    {
        public string CreateToken(ApplicationUser user);
    }
}
