using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.ShowTime
{
    public class ShowtimeCreateDTO
    {
        [Required(ErrorMessage = "Phim là bắt buộc.")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Ngày chiếu là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc là bắt buộc.")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Định dạng phim là bắt buộc.")]
        [StringLength(10, ErrorMessage = "Định dạng không được vượt quá 10 ký tự.")]
        public string Format { get; set; } = "2D";
    }
}
