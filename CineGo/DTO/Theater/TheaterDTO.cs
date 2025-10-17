using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO
{
    public class TheaterDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phòng chiếu không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên phòng chiếu không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = null!;

        [Range(1, 50, ErrorMessage = "Số hàng ghế phải nằm trong khoảng từ 1 đến 50.")]
        public int Rows { get; set; }

        [Range(1, 50, ErrorMessage = "Số cột ghế phải nằm trong khoảng từ 1 đến 50.")]
        public int Columns { get; set; }

        [Required(ErrorMessage = "Rạp chiếu không được để trống.")]
        public int CinemaId { get; set; }
    }
}
