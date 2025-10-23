using CineGo.DTO.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CineGo.Services.Helpers
{
    public static class SecurityHelper
    {
        // Hàm băm mật khẩu bằng SHA256
        public static string HashPassword(string password)
        {
            if (password is null)
                throw new ArgumentNullException(nameof(password), "Password không được null.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password không được rỗng hoặc toàn khoảng trắng.", nameof(password));

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        // Hàm tạo JWT token
        public static string GenerateJwtToken(UserResponseDTO user, string secretKey, int expireHours = 2)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(expireHours),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
