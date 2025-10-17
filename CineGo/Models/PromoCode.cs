using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGo.Models
{
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(IsActive))]
    [Index(nameof(ValidFrom))]
    [Index(nameof(ValidTo))]
    [Index(nameof(Code), nameof(IsActive))]
    public class PromoCode
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(50, ErrorMessage = "Mã giảm giá không được vượt quá 50 ký tự")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Loại giảm giá không được để trống")]
        [StringLength(20, ErrorMessage = "Loại giảm giá không được vượt quá 20 ký tự")]
        public string DiscountType { get; set; } = "Percent";
        // "Percent" hoặc "Fixed"

        [StringLength(250, ErrorMessage = "Mô tả không được vượt quá 250 ký tự")]
        public string? Description { get; set; }

        [Range(0, 1000000, ErrorMessage = "Giá trị giảm giá phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.Date)]
        public DateTime ValidFrom { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.Date)]
        public DateTime ValidTo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượt sử dụng phải lớn hơn hoặc bằng 0")]
        public int UsageCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        // Quan hệ 1:N — Một mã giảm giá có thể được dùng trong nhiều Order
        public ICollection<Order>? Orders { get; set; }

        // Hàm tiện ích kiểm tra còn hạn hay không
        [NotMapped]
        public bool IsValid => IsActive && DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidTo;
    }
}
