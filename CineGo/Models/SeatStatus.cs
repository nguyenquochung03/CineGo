using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    public class SeatStatus
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Trạng thái ghế không được để trống")]
        [StringLength(20, ErrorMessage = "Trạng thái ghế không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Available";

        // Người đang giữ ghế tạm (nếu có)
        [ForeignKey(nameof(User))]
        public int? LockedBy { get; set; }
        public User? User { get; set; }

        public DateTime? LockExpiresAt { get; set; }

        // Ghế cụ thể (1 suất chiếu có nhiều SeatStatus cho các ghế khác nhau)
        [Required]
        [ForeignKey(nameof(Seat))]
        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;

        // Suất chiếu mà ghế này thuộc về
        [Required]
        [ForeignKey(nameof(Showtime))]
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;
    }
}
