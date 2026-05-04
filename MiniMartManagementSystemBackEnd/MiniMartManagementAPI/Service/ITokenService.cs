using System.Security.Claims;

namespace MiniMartManagementAPI.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim>clains);
        string GenerateRefreshToken();
        ClaimsPrincipal PrincipalFromExpiredToken(string token); //phương thức này sẽ lấy thông tin từ token đã hết hạn để tạo ra một ClaimsPrincipal, từ đó có thể sử dụng thông tin này để tạo ra một access token mới
    }
}
