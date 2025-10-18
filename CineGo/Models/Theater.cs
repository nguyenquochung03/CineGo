using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(CinemaId))]
    [Index(nameof(Name), nameof(CinemaId), IsUnique = true)]
    public class Theater
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phòng chiếu không được để trống")]
        [StringLength(100, ErrorMessage = "Tên phòng chiếu không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [Range(1, 50, ErrorMessage = "Số hàng ghế phải nằm trong khoảng từ 1 đến 50")]
        public int Rows { get; set; }

        [Range(1, 50, ErrorMessage = "Số cột ghế phải nằm trong khoảng từ 1 đến 50")]
        public int Columns { get; set; }

        // Khóa ngoại đến Cinema
        [Required]
        [ForeignKey(nameof(Cinema))]
        public int CinemaId { get; set; }

        // Quan hệ N:1 — Một Theater thuộc về một Cinema
        public Cinema Cinema { get; set; } = null!;

        // Quan hệ 1:N — Một Theater có nhiều Seat
        public ICollection<Seat>? Seats { get; set; }

        // Quan hệ N:N với Showtime
        public ICollection<TheaterShowtime> TheaterShowtimes { get; set; } = new List<TheaterShowtime>();
    }
}
