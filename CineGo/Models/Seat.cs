using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(TheaterId))]
    [Index(nameof(Row))]
    [Index(nameof(Column))]
    [Index(nameof(Label))]
    [Index(nameof(TheaterId), nameof(Row), nameof(Column), IsUnique = true)]
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số hàng phải lớn hơn 0")]
        public int Row { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số cột phải lớn hơn 0")]
        public int Column { get; set; }

        [Required(ErrorMessage = "Loại ghế không được để trống")]
        [StringLength(50, ErrorMessage = "Loại ghế không được vượt quá 50 ký tự")]
        public string Type { get; set; } = "Standard";

        [Required(ErrorMessage = "Nhãn ghế không được để trống")]
        [StringLength(10, ErrorMessage = "Nhãn ghế không được vượt quá 10 ký tự")]
        public string Label { get; set; } = null!;

        // Quan hệ 1:N — Một rạp (Theater) có nhiều ghế
        [Required]
        [ForeignKey(nameof(Theater))]
        public int TheaterId { get; set; }
        public Theater Theater { get; set; } = null!;

        // 🔗 Quan hệ 1:N — Một ghế có nhiều trạng thái ghế ở các suất chiếu khác nhau
        public ICollection<SeatStatus>? SeatStatuses { get; set; }
    }
}
