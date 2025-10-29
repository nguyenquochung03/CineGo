using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.User
{
    public class UserLoginUserDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập email hoặc số điện thoại.")]
        public string EmailOrPhone { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; } = null!;
    }
}
