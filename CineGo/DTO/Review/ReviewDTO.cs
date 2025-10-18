using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.Review
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nội dung đánh giá không được để trống.")]
        [StringLength(2000, ErrorMessage = "Nội dung đánh giá không được vượt quá 2000 ký tự.")]
        public string Content { get; set; } = null!;

        [Range(0, 10, ErrorMessage = "Điểm đánh giá phải từ 0 đến 10.")]
        public int Rating { get; set; }
    }
}
