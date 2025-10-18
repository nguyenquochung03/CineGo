using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.ShowTime
{
    public class ShowtimeUpdateDTO : ShowtimeCreateDTO
    {
        [Required(ErrorMessage = "Id là bắt buộc.")]
        public int Id { get; set; }
    }
}
