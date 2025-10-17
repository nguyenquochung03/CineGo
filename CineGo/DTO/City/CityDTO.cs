using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO
{
    public class CityDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thành phố không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên thành phố không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vùng không được để trống.")]
        public int RegionId { get; set; }
    }
}
