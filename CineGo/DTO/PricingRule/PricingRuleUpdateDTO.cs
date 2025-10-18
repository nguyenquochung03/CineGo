using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.PricingRule
{
    public class PricingRuleUpdateDTO : PricingRuleCreateDTO
    {
        [Required(ErrorMessage = "Id không được để trống khi cập nhật.")]
        public int Id { get; set; }
    }
}
