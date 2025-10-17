using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CineGo.Models
{
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(IsActive))]
    [Index(nameof(CreatedAt))]
    public class PricingRule
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên quy tắc giá không được để trống")]
        [StringLength(100, ErrorMessage = "Tên quy tắc giá không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(250, ErrorMessage = "Mô tả không được vượt quá 250 ký tự")]
        public string? Description { get; set; }

        // Thời lượng hiệu lực (theo phút hoặc ngày, tuỳ ý định nghĩa)
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị Runtime phải lớn hơn hoặc bằng 0")]
        public int Runtime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Quan hệ 1:N — Một PricingRule có nhiều ApplicableDays
        public ICollection<PricingRuleDay>? ApplicableDays { get; set; }

        // Quan hệ 1:N — Một PricingRule có nhiều PricingDetail
        public ICollection<PricingDetail>? PricingDetails { get; set; }

        // Quan hệ 1:N — Một PricingRule có thể áp dụng cho nhiều Showtime
        public ICollection<Showtime>? Showtimes { get; set; }
    }
}
