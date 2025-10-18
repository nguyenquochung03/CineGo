using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.PricingDetail
{
    public class PricingDetailUpdateDTO : PricingDetailCreateDTO
    {
        [Required(ErrorMessage = "Id không được để trống khi cập nhật.")]
        public int Id { get; set; }
    }
}
