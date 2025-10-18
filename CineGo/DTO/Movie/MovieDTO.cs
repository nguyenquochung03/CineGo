using CineGo.DTO.MoviePoster;
using CineGo.DTO.Review;
using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.Movie
{
    public class MovieDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phim không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên phim không được vượt quá 200 ký tự.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Slug không được để trống.")]
        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự.")]
        public string Slug { get; set; } = null!;

        [Range(1, 600, ErrorMessage = "Thời lượng phim phải từ 1 đến 600 phút.")]
        public int Runtime { get; set; }

        [Required(ErrorMessage = "Xếp hạng phim không được để trống.")]
        [StringLength(10, ErrorMessage = "Xếp hạng phim không được vượt quá 10 ký tự.")]
        public string Rating { get; set; } = null!;

        [Range(0, 21, ErrorMessage = "Giới hạn tuổi phải từ 0 đến 21.")]
        public int AgeLimit { get; set; }

        [Required(ErrorMessage = "Ngày phát hành không được để trống.")]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Tóm tắt phim không được để trống.")]
        [StringLength(2000, ErrorMessage = "Tóm tắt phim không được vượt quá 2000 ký tự.")]
        public string Synopsis { get; set; } = null!;

        [Url(ErrorMessage = "Trailer phải là URL hợp lệ.")]
        public string? TrailerUrl { get; set; }

        public List<MoviePosterDTO> Posters { get; set; } = new();
        public List<ReviewDTO> Reviews { get; set; } = new();
    }
}