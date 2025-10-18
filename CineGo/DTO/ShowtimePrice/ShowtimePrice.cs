namespace CineGo.DTO.ShowtimePrice
{
    public class ShowtimePriceDTO
    {
        public int Id { get; set; }
        public int ShowtimeId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
