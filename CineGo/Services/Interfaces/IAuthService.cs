using CineGo.DTO.User;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> AuthenticateAdminAsync(UserLoginDTO loginDto, string jwtSecret);
        Task<ApiResponse> AuthenticateUserForClientAsync(UserLoginUserDTO loginDto, string jwtSecret);
        Task<ApiResponse> RegisterAdminAsync(UserRegisterDTO registerDto);
        Task<ApiResponse> CheckUserExistsAsync(string? email, string? phone);
        Task<ApiResponse> RegisterUserAsync(UserRegisterDTO registerDto);
        Task<ApiResponse> VerifyByEmailAsync(string email, string code, string jwtSecret);
        Task<ApiResponse> VerifyBySmsAsync(string phone, string code, string jwtSecret);
        Task<ApiResponse> ResendOtpByEmailAsync(string email);
        Task<ApiResponse> ResendOtpBySmsAsync(string phone);
    }
}
