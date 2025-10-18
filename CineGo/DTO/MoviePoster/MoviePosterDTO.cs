using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.MoviePoster
{
    public class MoviePosterDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh không được để trống.")]
        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự.")]
        public string Url { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn hoặc bằng 0.")]
        public int Order { get; set; } = 0;
    }
}
