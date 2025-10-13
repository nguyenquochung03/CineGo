using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    public class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên rạp chiếu phim không được để trống")]
        [StringLength(150, ErrorMessage = "Tên rạp chiếu phim không được vượt quá 150 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(250, ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự")]
        public string Address { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Tọa độ không được vượt quá 100 ký tự")]
        public string? Coords { get; set; }

        [StringLength(1000, ErrorMessage = "Thông tin tiện ích không được vượt quá 1000 ký tự")]
        public string? Amenities { get; set; }

        // Khóa ngoại đến City
        [Required]
        [ForeignKey(nameof(City))]
        public int CityId { get; set; }

        // Quan hệ N:1 - Một rạp thuộc về một thành phố
        public City City { get; set; } = null!;

        // Quan hệ 1:N - Một rạp có nhiều phòng chiếu (Theater)
        public ICollection<Theater>? Theaters { get; set; }
    }
}
