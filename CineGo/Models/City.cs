using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(Name), nameof(RegionId), IsUnique = true)]
    [Index(nameof(RegionId))]
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thành phố không được để trống")]
        [StringLength(100, ErrorMessage = "Tên thành phố không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        // Khóa ngoại đến Region
        [Required]
        [ForeignKey(nameof(Region))]
        public int RegionId { get; set; }

        // Quan hệ N:1 - Một City thuộc về một Region
        public Region Region { get; set; } = null!;

        // Quan hệ 1:N - Một City có nhiều Cinema
        public ICollection<Cinema>? Cinemas { get; set; }
    }
}
