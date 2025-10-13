using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Dữ liệu QR không được để trống")]
        [StringLength(500, ErrorMessage = "Dữ liệu QR không được vượt quá 500 ký tự")]
        public string QRCodeData { get; set; } = null!;

        [Required(ErrorMessage = "Trạng thái vé không được để trống")]
        [StringLength(20, ErrorMessage = "Trạng thái vé không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Active";

        [Required(ErrorMessage = "Mã vé không được để trống")]
        [StringLength(50, ErrorMessage = "Mã vé không được vượt quá 50 ký tự")]
        public string TicketCode { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(OrderItem))]
        public int OrderItemId { get; set; }

        // Quan hệ 1:1 — Mỗi vé tương ứng với một OrderItem
        public OrderItem OrderItem { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? UsedAt { get; set; }
    }
}
