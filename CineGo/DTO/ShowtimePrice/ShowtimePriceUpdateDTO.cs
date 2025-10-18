using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.ShowtimePrice
{
    public class ShowtimePriceUpdateDTO : ShowtimePriceCreateDTO
    {
        [Required(ErrorMessage = "Id là bắt buộc.")]
        public int Id { get; set; }
    }
}
