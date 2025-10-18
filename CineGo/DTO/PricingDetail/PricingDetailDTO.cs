using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.PricingDetail
{
    public class PricingDetailDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Loại vé không được để trống.")]
        [StringLength(50, ErrorMessage = "Loại vé không được vượt quá 50 ký tự.")]
        public string TicketType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại ghế không được để trống.")]
        [StringLength(50, ErrorMessage = "Loại ghế không được vượt quá 50 ký tự.")]
        public string SeatType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá cơ bản không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá cơ bản phải lớn hơn hoặc bằng 0.")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Quy tắc giá không được để trống.")]
        public int PricingRuleId { get; set; }
    }
}