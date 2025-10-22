namespace CineGo.DTO.PromoCode
{
    public class PromoCodeDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Value { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid => IsActive && DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidTo;
    }
}
