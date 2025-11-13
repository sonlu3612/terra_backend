using Core.Entities;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(ApplicationUser user);
    }
}