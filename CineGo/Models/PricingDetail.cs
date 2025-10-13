using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    public class PricingDetail
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Loại vé không được để trống")]
        [StringLength(50, ErrorMessage = "Loại vé không được vượt quá 50 ký tự")]
        public string TicketType { get; set; } = null!;

        [Required(ErrorMessage = "Loại ghế không được để trống")]
        [StringLength(50, ErrorMessage = "Loại ghế không được vượt quá 50 ký tự")]
        public string SeatType { get; set; } = null!;

        [Required(ErrorMessage = "Giá cơ bản không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá cơ bản phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; }

        // Khóa ngoại đến PricingRule
        [Required]
        [ForeignKey(nameof(PricingRule))]
        public int PricingRuleId { get; set; }

        // Quan hệ N:1 — Một chi tiết giá thuộc về một quy tắc giá
        public PricingRule PricingRule { get; set; } = null!;
    }
}
