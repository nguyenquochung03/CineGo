using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO
{
    public class RegionDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên vùng không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên vùng không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = null!;
    }
}
