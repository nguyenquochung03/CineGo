using CineGo.DTO.User;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineGo.Controllers
{
    [Route("[controller]/[action]")]
    public class UserAuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly string _jwtSecret;

        public UserAuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _jwtSecret = config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string? emailOrPhone = null)
        {
            var token = HttpContext.Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                try
                {
                    var key = Encoding.ASCII.GetBytes(_jwtSecret);
                    jwtHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    return RedirectToAction("Index", "MovieList");
                }
                catch
                {
                    Response.Cookies.Delete("jwt");
                }
            }

            if (HttpContext.Request.Query["expired"] == "true")
                ViewBag.AlertMessage = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";

            ViewBag.EmailOrPhone = emailOrPhone;
            return View("LoginOrRegister");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginUserDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            // Gọi service để xác thực và tạo token
            var result = await _authService.AuthenticateUserForClientAsync(loginDto, _jwtSecret);

            if (!result.Success && result.Status == 403)
            {
                string emailOrPhone = result.Data?.ToString()!;
                ViewBag.EmailOrPhone = emailOrPhone;

                // Gửi OTP mới nếu muốn
                await _authService.ResendOtpByEmailAsync(emailOrPhone);

                return View("VerifyOtp");
            }


            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewBag.EmailOrPhone = loginDto.EmailOrPhone;
                return View();
            }

            // Kiểm tra Data null trước khi dùng
            if (result.Data is not null)
            {
                // Dùng dynamic an toàn hơn
                var data = result.Data as dynamic;

                string? token = data?.Token;
                UserResponseDTO? user = data?.User as UserResponseDTO;

                if (string.IsNullOrEmpty(token) || user is null)
                {
                    ViewBag.Error = "Thông tin đăng nhập không hợp lệ.";
                    return View();
                }

                // Tạo cookie HttpOnly
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(2)
                });

                // Lưu thông tin admin
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);

                return RedirectToAction("Index", "MovieList");
            }

            ViewBag.Error = "Dữ liệu đăng nhập trống.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckUserExists(string emailOrPhone)
        {
            if (string.IsNullOrWhiteSpace(emailOrPhone))
                return View("LoginOrRegister", new { Error = "Vui lòng nhập email hoặc số điện thoại." });

            bool isEmail = Regex.IsMatch(emailOrPhone, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            bool isPhone = Regex.IsMatch(emailOrPhone, @"^(0|\+84)(\d{9})$");

            if (!isEmail && !isPhone)
            {
                ViewBag.Error = "Định dạng không hợp lệ. Vui lòng nhập đúng email hoặc số điện thoại.";
                return View("LoginOrRegister");
            }

            var response = await _authService.CheckUserExistsAsync(isEmail ? emailOrPhone : null, isPhone ? emailOrPhone : null);

            if (!response.Success)
            {
                ViewBag.Error = response.Message;
                return View("LoginOrRegister");
            }

            ViewBag.EmailOrPhone = emailOrPhone;

            if ((bool)response.Data!)
                return View("Login");
            else
                return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            var result = await _authService.RegisterUserAsync(dto);

            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewBag.EmailOrPhone = !string.IsNullOrWhiteSpace(dto.Email) ? dto.Email : dto.Phone;
                return View("Register");
            }

            ViewBag.EmailOrPhone = !string.IsNullOrWhiteSpace(dto.Email) ? dto.Email : dto.Phone;
            return View("VerifyOtp");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string emailOrPhone, string code)
        {
            bool isEmail = emailOrPhone.Contains("@");
            ApiResponse result;

            if (isEmail)
                result = await _authService.VerifyByEmailAsync(emailOrPhone, code, _jwtSecret);
            else
                result = await _authService.VerifyBySmsAsync(emailOrPhone, code, _jwtSecret);

            ViewBag.EmailOrPhone = emailOrPhone;

            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View("VerifyOtp");
            }

            // Lấy token và user từ ApiResponse
            var data = result.Data as dynamic;
            string? token = data?.Token;
            UserResponseDTO? user = data?.User as UserResponseDTO;

            if (string.IsNullOrEmpty(token) || user is null)
            {
                ViewBag.Error = "Thông tin đăng nhập không hợp lệ.";
                return View("VerifyOtp");
            }

            // Tạo cookie HttpOnly
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            // Lưu thông tin người dùng vào session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);

            // Redirect sang MovieList
            return RedirectToAction("Index", "MovieList");
        }

        [HttpPost]
        public async Task<IActionResult> ResendOtp(string emailOrPhone)
        {
            if (string.IsNullOrWhiteSpace(emailOrPhone))
            {
                ViewBag.Error = "Không có thông tin email hoặc số điện thoại.";
                return View("VerifyOtp");
            }

            bool isEmail = Regex.IsMatch(emailOrPhone, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            var result = isEmail
                ? await _authService.ResendOtpByEmailAsync(emailOrPhone)
                : await _authService.ResendOtpBySmsAsync(emailOrPhone);

            ViewBag.EmailOrPhone = emailOrPhone;

            if (!result.Success)
                ViewBag.Error = result.Message;
            else
                ViewBag.Message = result.Message;

            return View("VerifyOtp");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "MovieList");
        }
    }
}
