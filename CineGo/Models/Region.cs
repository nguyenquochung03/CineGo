using System.ComponentModel.DataAnnotations;

namespace CineGo.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên vùng không được để trống")]
        [StringLength(100, ErrorMessage = "Tên vùng không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        // Quan hệ 1:N - Một Region có nhiều City
        public ICollection<City>? Cities { get; set; }
    }
}
