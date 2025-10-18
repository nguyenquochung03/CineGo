using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.ShowtimePrice
{
    public class ShowtimePriceCreateDTO
    {
        [Required(ErrorMessage = "ShowtimeId là bắt buộc.")]
        public int ShowtimeId { get; set; }

        [Required(ErrorMessage = "Loại vé là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Loại vé không được vượt quá 50 ký tự.")]
        public string TicketType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại ghế là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Loại ghế không được vượt quá 50 ký tự.")]
        public string SeatType { get; set; } = string.Empty;

        [Range(0, 1000000, ErrorMessage = "Giá vé phải nằm trong khoảng từ 0 đến 1,000,000.")]
        public decimal Price { get; set; }
    }
}
