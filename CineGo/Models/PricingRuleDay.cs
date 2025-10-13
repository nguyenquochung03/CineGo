using CineGo.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CineGo.Models
{
    public class PricingRuleDay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(PricingRule))]
        public int PricingRuleId { get; set; }
        public PricingRule PricingRule { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string DayName { get; set; } = null!;
    }
}