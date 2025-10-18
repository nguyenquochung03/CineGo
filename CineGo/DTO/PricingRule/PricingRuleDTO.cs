using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.PricingRule
{
    public class PricingRuleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên quy tắc giá không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên quy tắc giá không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Mô tả không được vượt quá 250 ký tự.")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Runtime phải lớn hơn hoặc bằng 0.")]
        public int Runtime { get; set; }

        [Required(ErrorMessage = "Trạng thái hoạt động không được để trống.")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Ngày tạo không được để trống.")]
        public DateTime CreatedAt { get; set; }
    }
}
