using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá vé phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Loại vé không được để trống")]
        [StringLength(50, ErrorMessage = "Loại vé không được vượt quá 50 ký tự")]
        public string TicketType { get; set; } = null!;

        // Quan hệ N:1 — Một Order có nhiều OrderItem
        [Required]
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // 🔗 Quan hệ 1:1 — Mỗi OrderItem có thể sinh ra một vé điện tử
        public Ticket? Ticket { get; set; }
    }
}
