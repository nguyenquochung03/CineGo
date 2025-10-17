using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(MovieId))]
    [Index(nameof(UserId))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(Rating))]
    [Index(nameof(MovieId), nameof(UserId), IsUnique = true)]
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 10, ErrorMessage = "Điểm đánh giá phải nằm trong khoảng từ 1 đến 10")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Nội dung đánh giá không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung đánh giá không được vượt quá 1000 ký tự")]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Người dùng viết đánh giá
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Phim được đánh giá
        [Required]
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
