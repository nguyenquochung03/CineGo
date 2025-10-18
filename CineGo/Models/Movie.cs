using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(Title))]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(ReleaseDate))]
    [Index(nameof(Rating))]
    [Index(nameof(AgeLimit))]
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phim không được để trống")]
        [StringLength(200, ErrorMessage = "Tên phim không được vượt quá 200 ký tự")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Slug không được để trống")]
        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự")]
        public string Slug { get; set; } = null!;

        [Range(1, 600, ErrorMessage = "Thời lượng phim phải từ 1 đến 600 phút")]
        public int Runtime { get; set; }

        [Required(ErrorMessage = "Xếp hạng phim không được để trống")]
        [StringLength(10, ErrorMessage = "Xếp hạng phim không được vượt quá 10 ký tự")]
        public string Rating { get; set; } = null!;

        [Range(0, 21, ErrorMessage = "Giới hạn tuổi phải từ 0 đến 21")]
        public int AgeLimit { get; set; }

        [Required(ErrorMessage = "Ngày phát hành không được để trống")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Tóm tắt phim không được để trống")]
        [StringLength(2000, ErrorMessage = "Tóm tắt phim không được vượt quá 2000 ký tự")]
        public string Synopsis { get; set; } = null!;

        [Url(ErrorMessage = "Trailer phải là đường dẫn hợp lệ")]
        public string? TrailerUrl { get; set; }

        // Danh sách các poster của phim
        public ICollection<MoviePoster>? Posters { get; set; }

        // Một phim có thể có nhiều suất chiếu
        public ICollection<Showtime>? Showtimes { get; set; }

        // Một phim có thể có nhiều đánh giá
        public ICollection<Review>? Reviews { get; set; }
    }
}
