using CineGo.DTO.ShowtimePrice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CineGo.DTO.Showtime
{
    // Dùng khi hiển thị dữ liệu showtime
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Format { get; set; } = string.Empty;
        public int PricingRuleId { get; set; }
        public bool IsWeekend { get; set; }
    }
}
