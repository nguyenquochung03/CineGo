using CineGo.DTO.User;
using CineGo.Models;
using CineGo.Services.Interfaces;
using CineGo.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly CineGoDbContext _context;

        public AuthService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> AuthenticateAdminAsync(UserLoginDTO loginDto, string jwtSecret)
        {
            string hashed = SecurityHelper.HashPassword(loginDto.Password);

            var admin = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == loginDto.Email &&
                u.HashedPassword == hashed &&
                u.Role == "Admin");

            if (admin == null)
                return ApiResponse.ErrorResponse(401, "Email hoặc mật khẩu không đúng.");

            var response = new UserResponseDTO
            {
                Id = admin.Id,
                Name = admin.Name,
                Email = admin.Email,
                Role = admin.Role
            };

            var token = SecurityHelper.GenerateJwtToken(response, jwtSecret);

            return ApiResponse.SuccessResponse(new { Token = token, User = response }, "Đăng nhập thành công.");
        }

        public async Task<ApiResponse> AuthenticateUserAsync(UserLoginDTO loginDto, string jwtSecret)
        {
            string hashed = SecurityHelper.HashPassword(loginDto.Password);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == loginDto.Email &&
                u.HashedPassword == hashed &&
                u.Role == "User");

            if (user == null)
                return ApiResponse.ErrorResponse(401, "Email hoặc mật khẩu không đúng.");

            var response = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            var token = SecurityHelper.GenerateJwtToken(response, jwtSecret);

            return ApiResponse.SuccessResponse(new { Token = token, User = response }, "Đăng nhập thành công.");
        }

        public async Task<ApiResponse> RegisterAdminAsync(UserRegisterDTO registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return ApiResponse.ErrorResponse(400, "Email đã tồn tại.");

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                HashedPassword = SecurityHelper.HashPassword(registerDto.Password),
                Role = "Admin",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return ApiResponse.SuccessResponse(response, "Đăng ký thành công.");
        }
    }
}
