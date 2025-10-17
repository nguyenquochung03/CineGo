using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(MovieId))]
    [Index(nameof(MovieId), nameof(Order), IsUnique = true)]
    public class MoviePoster
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh không được để trống")]
        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự")]
        public string Url { get; set; } = null!;

        // Thứ tự hiển thị (0 = mặc định)
        [Range(0, int.MaxValue)]
        public int Order { get; set; } = 0;

        // 🔗 Quan hệ đến Movie
        [Required]
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
