using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO
{
    public class CinemaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên rạp chiếu không được để trống.")]
        [StringLength(150, ErrorMessage = "Tên rạp chiếu không được vượt quá 150 ký tự.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(250, ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Thành phố không được để trống.")]
        public int CityId { get; set; }
        public string? Amenities { get; set; }
    }
}
