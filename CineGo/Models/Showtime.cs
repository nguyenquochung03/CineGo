using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(TheaterId))]
    [Index(nameof(MovieId))]
    [Index(nameof(PricingRuleId))]
    [Index(nameof(Date), nameof(TheaterId))]
    [Index(nameof(Date), nameof(MovieId))]
    [Index(nameof(TheaterId), nameof(MovieId), nameof(Date), nameof(StartTime), IsUnique = true)]
    public class Showtime
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ngày chiếu không được để trống")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Giờ bắt đầu không được để trống")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc không được để trống")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Định dạng phim không được để trống")]
        [StringLength(10, ErrorMessage = "Định dạng phim không được vượt quá 10 ký tự")]
        public string Format { get; set; } = "2D";

        public bool IsWeekend { get; set; }

        // Quan hệ đến Theater
        [Required]
        [ForeignKey(nameof(Theater))]
        public int TheaterId { get; set; }
        public Theater Theater { get; set; } = null!;

        // Quan hệ đến Movie
        [Required]
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        // Quan hệ đến PricingRule (quy tắc giá)
        [Required]
        [ForeignKey(nameof(PricingRule))]
        public int PricingRuleId { get; set; }
        public PricingRule PricingRule { get; set; } = null!;

        // Quan hệ 1:N — Một suất chiếu có nhiều Order (vé đặt)
        public ICollection<Order>? Orders { get; set; }

        // Quan hệ 1:N — Một suất chiếu có nhiều ShowtimePrice (giá theo loại ghế)
        public ICollection<ShowtimePrice>? ShowtimePrices { get; set; }

        // Quan hệ 1:N — Một suất chiếu có nhiều trạng thái ghế (SeatStatuses)
        public ICollection<SeatStatus>? SeatStatuses { get; set; }
    }
}
