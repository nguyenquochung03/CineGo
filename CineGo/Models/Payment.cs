using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(OrderId))]
    [Index(nameof(Status))]
    [Index(nameof(Method))]
    [Index(nameof(ProviderTxnId), IsUnique = false)]
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán không được để trống")]
        [StringLength(50, ErrorMessage = "Phương thức thanh toán không được vượt quá 50 ký tự")]
        public string Method { get; set; } = null!;

        [Required]
        [StringLength(20, ErrorMessage = "Trạng thái thanh toán không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Pending";

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ProcessedAt { get; set; }
        // Ngày thanh toán hoàn tất (nếu có)

        [StringLength(100, ErrorMessage = "Mã giao dịch từ nhà cung cấp không được vượt quá 100 ký tự")]
        public string? ProviderTxnId { get; set; }

        // Quan hệ N:1 — Một Payment thuộc về một Order
        [Required]
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
