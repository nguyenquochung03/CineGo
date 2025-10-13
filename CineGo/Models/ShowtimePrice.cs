using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    public class ShowtimePrice
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Loại vé không được để trống")]
        [StringLength(50, ErrorMessage = "Loại vé không được vượt quá 50 ký tự")]
        public string TicketType { get; set; } = null!;

        [Required(ErrorMessage = "Loại ghế không được để trống")]
        [StringLength(50, ErrorMessage = "Loại ghế không được vượt quá 50 ký tự")]
        public string SeatType { get; set; } = null!; 

        [Range(0, 1000000, ErrorMessage = "Giá vé phải nằm trong khoảng hợp lệ")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Quan hệ đến Showtime (1 suất chiếu có nhiều giá vé)
        [Required]
        [ForeignKey(nameof(Showtime))]
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;
    }
}
