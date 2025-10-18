using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.PricingRuleDay
{
    public class PricingRuleDayUpdateDTO : PricingRuleDayCreateDTO
    {
        [Required(ErrorMessage = "Id không được để trống khi cập nhật.")]
        public int Id { get; set; }
    }
}
