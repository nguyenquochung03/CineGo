using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Trạng thái đơn hàng không được để trống")]
        [StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Quan hệ 1:N — Một User có thể có nhiều Order
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Quan hệ 1:N — Một Showtime có thể có nhiều Order
        [Required]
        [ForeignKey(nameof(Showtime))]
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;

        // Quan hệ N:1 — Một Order có thể áp dụng một mã giảm giá (PromoCode)
        [ForeignKey(nameof(PromoCode))]
        public int? PromoCodeId { get; set; }
        public PromoCode? PromoCode { get; set; }

        // Quan hệ 1:N — Một Order có thể có nhiều vé (OrderItem)
        public ICollection<OrderItem>? OrderItems { get; set; }

        // Quan hệ 1:N — Một Order có thể có nhiều lần thanh toán (Payment)
        public ICollection<Payment>? Payments { get; set; }
    }
}
