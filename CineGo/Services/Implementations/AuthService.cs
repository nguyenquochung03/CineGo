using Azure;
using CineGo.DTO.User;
using CineGo.Models;
using CineGo.Services.Helpers;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CineGo.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly CineGoDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        // Bộ nhớ tạm lưu mã xác thực (email/phone → code)
        private static readonly ConcurrentDictionary<string, OtpEntry> _otpStore = new();

        public AuthService(CineGoDbContext context, IEmailService emailService, ISmsService smsService)
        {
            _context = context;
            _emailService = emailService;
            _smsService = smsService;
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

        public async Task<ApiResponse> AuthenticateUserForClientAsync(UserLoginUserDTO loginDto, string jwtSecret)
        {
            string hashed = SecurityHelper.HashPassword(loginDto.Password);
            bool isEmail = loginDto.EmailOrPhone.Contains("@");

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                (isEmail ? u.Email == loginDto.EmailOrPhone : u.Phone == loginDto.EmailOrPhone) &&
                u.HashedPassword == hashed &&
                u.Role == "User");

            if (user == null)
                return ApiResponse.ErrorResponse(401, "Email/SĐT hoặc mật khẩu không đúng.");

            if (!user.IsVerified)
            {
                return ApiResponse.ErrorResponse(403, "Tài khoản chưa xác thực.", user.Email ?? user.Phone);
            }

            var response = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
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

        public async Task<ApiResponse> CheckUserExistsAsync(string? email, string? phone)
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                return ApiResponse.ErrorResponse(400, "Phải cung cấp email hoặc số điện thoại.");

            bool exists = false;

            if (!string.IsNullOrWhiteSpace(email))
            {
                exists = await _context.Users.AnyAsync(u => u.Email == email && u.Role == "User");
            }
            else if (!string.IsNullOrWhiteSpace(phone))
            {
                exists = await _context.Users.AnyAsync(u => u.Phone == phone && u.Role == "User");
            }

            if (exists)
                return ApiResponse.SuccessResponse(true, "Người dùng đã tồn tại.");
            else
                return ApiResponse.SuccessResponse(false, "Người dùng chưa tồn tại.");
        }

        public async Task<ApiResponse> RegisterUserAsync(UserRegisterDTO registerDto)
        {
            bool hasEmail = !string.IsNullOrWhiteSpace(registerDto.Email);
            bool hasPhone = !string.IsNullOrWhiteSpace(registerDto.Phone);

            // Kiểm tra đầu vào
            if (!hasEmail && !hasPhone)
                return ApiResponse.ErrorResponse(400, "Phải có ít nhất email hoặc số điện thoại.");

            // Kiểm tra trùng
            if (hasEmail)
            {
                bool existsEmail = await _context.Users.AnyAsync(u => hasEmail && u.Email == registerDto.Email && u.Role == "User");

                if (existsEmail)
                    return ApiResponse.ErrorResponse(400, $"Email {registerDto.Email} đã được sử dụng.");
            }

            if (hasPhone)
            {
                bool existsPhone = await _context.Users.AnyAsync(u => hasPhone && u.Phone == registerDto.Phone && u.Role == "User");
                if (existsPhone)
                    return ApiResponse.ErrorResponse(400, $"Số điện thoại {registerDto.Phone} đã được sử dụng.");
            }

            // Sinh mã OTP
            string otp = new Random().Next(1000, 9999).ToString();
            string key = hasEmail ? registerDto.Email! : registerDto.Phone!;

            _otpStore[key] = new OtpEntry
            {
                Code = otp,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            };

            // Gửi OTP
            if (hasEmail)
            {
                string subject = "Xác thực tài khoản CineGo";
                string body = $@"
                    <h3>Xin chào {registerDto.Name},</h3>
                    <p>Mã xác thực của bạn là: <b>{otp}</b></p>
                    <p>Mã có hiệu lực trong 5 phút.</p>";
                await _emailService.SendEmailAsync(registerDto.Email!, subject, body);
            }
            else
            {
                await _smsService.SendSmsAsync(registerDto.Phone!, $"Mã xác thực CineGo của bạn là: {otp}");
            }

            // Lưu user chưa xác thực
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                HashedPassword = SecurityHelper.HashPassword(registerDto.Password),
                Role = "User",
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (hasEmail)
            {
                return ApiResponse.SuccessResponse(null, "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.");
            }
            else
            {
                return ApiResponse.SuccessResponse(null, "Đăng ký thành công. Vui lòng kiểm tra tin nhắn SMS để xác thực tài khoản.");
            }
        }
            
        public async Task<ApiResponse> VerifyByEmailAsync(string email, string code, string jwtSecret)
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(code))
                return ApiResponse.ErrorResponse(400, "Email và mã xác thực không được để trống.");
            else if (string.IsNullOrWhiteSpace(email))
                return ApiResponse.ErrorResponse(400, "Email không được để trống.");
            else if (string.IsNullOrWhiteSpace(code))
                return ApiResponse.ErrorResponse(400, "Mã xác thực không được để trống.");

            // Kiểm tra OTP trong bộ nhớ tạm
            if (!_otpStore.TryGetValue(email, out var entry))
                return ApiResponse.ErrorResponse(400, "Mã xác thực không tồn tại hoặc đã hết hạn.");

            // Kiểm tra hết hạn
            if (entry.Expiration < DateTime.UtcNow)
            {
                _otpStore.TryRemove(email, out _);
                return ApiResponse.ErrorResponse(400, "Mã xác thực đã hết hạn.");
            }

            // Kiểm tra đúng code
            if (entry.Code != code)
                return ApiResponse.ErrorResponse(400, "Mã xác thực không hợp lệ.");

            // Lấy user theo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role == "User");
            if (user == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy tài khoản.");

            // Cập nhật trạng thái xác thực
            user.IsVerified = true;
            await _context.SaveChangesAsync();

            // Xóa OTP khỏi bộ nhớ
            _otpStore.TryRemove(email, out _);

            // Tạo token JWT cho phiên đăng nhập
            var response = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Role = user.Role
            };

            var token = SecurityHelper.GenerateJwtToken(response, jwtSecret);

            return ApiResponse.SuccessResponse(new {Token = token, User = response}, "Xác thực email thành công.");
        }

        public async Task<ApiResponse> VerifyBySmsAsync(string phone, string code, string jwtSecret)
        {
            // TODO: Sẽ cài đặt sau (khi có SMS provider thật)
            await Task.CompletedTask;
            return ApiResponse.ErrorResponse(501, "Chức năng xác minh qua SMS chưa được hỗ trợ.");
        }

        public async Task<ApiResponse> ResendOtpByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ApiResponse.ErrorResponse(400, "Email không được để trống.");

            // Kiểm tra email có tồn tại trong DB không
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role == "User");
            if (user == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy tài khoản với email này.");

            if (user.IsVerified)
                return ApiResponse.ErrorResponse(400, "Tài khoản đã được xác thực.");

            // Sinh OTP mới
            string otp = new Random().Next(1000, 9999).ToString();

            // Lưu OTP mới vào bộ nhớ tạm
            _otpStore[email] = new OtpEntry
            {
                Code = otp,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            };

            // Gửi OTP qua email
            string subject = "Xác thực lại tài khoản CineGo";
            string body = $@"
            <h3>Xin chào {user.Name},</h3>
            <p>Mã xác thực mới của bạn là: <b>{otp}</b></p>
            <p>Mã có hiệu lực trong 5 phút.</p>";

            await _emailService.SendEmailAsync(email, subject, body);

            return ApiResponse.SuccessResponse(null, "Mã xác thực mới đã được gửi. Vui lòng kiểm tra email.");
        }

        public async Task<ApiResponse> ResendOtpBySmsAsync(string phone)
        {
            await Task.CompletedTask;
            return ApiResponse.ErrorResponse(501, "Chức năng xác minh qua SMS chưa được hỗ trợ.");
        }
    }
}
