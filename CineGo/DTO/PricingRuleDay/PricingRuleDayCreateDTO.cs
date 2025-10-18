using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.PricingRuleDay
{
    public class PricingRuleDayCreateDTO
    {
        [Required(ErrorMessage = "Tên ngày không được để trống.")]
        [StringLength(20, ErrorMessage = "Tên ngày không được vượt quá 20 ký tự.")]
        public string DayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quy tắc giá không được để trống.")]
        public int PricingRuleId { get; set; }
    }
}
