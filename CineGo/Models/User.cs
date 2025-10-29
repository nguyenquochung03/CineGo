using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(Phone), IsUnique = true)]
    [Index(nameof(Name))]
    [Index(nameof(Role))]
    [Index(nameof(CreatedAt))]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên người dùng không được để trống")]
        [StringLength(100, ErrorMessage = "Tên người dùng không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255, ErrorMessage = "Mật khẩu mã hóa không được vượt quá 255 ký tự")]
        public string HashedPassword { get; set; } = null!;

        public bool IsVerified { get; set; } = false;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Quan hệ 1:N — Một User có thể khóa nhiều ghế (SeatStatus)
        public ICollection<SeatStatus>? LockedSeats { get; set; }

        // Quan hệ 1:N — Một User có thể có nhiều đơn đặt vé (Order)
        public ICollection<Order>? Orders { get; set; }

        // Quan hệ 1:N — Một User có thể viết nhiều đánh giá (Review)
        public ICollection<Review>? Reviews { get; set; }
    }
}
