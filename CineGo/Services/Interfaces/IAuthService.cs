using CineGo.DTO.User;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> AuthenticateAdminAsync(UserLoginDTO loginDto, string jwtSecret);
        Task<ApiResponse> AuthenticateUserAsync(UserLoginDTO loginDto, string jwtSecret);
        Task<ApiResponse> RegisterAdminAsync(UserRegisterDTO registerDto);
    }
}
