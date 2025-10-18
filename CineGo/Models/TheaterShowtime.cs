namespace CineGo.Models
{
    public class TheaterShowtime
    {
        public int TheaterId { get; set; }
        public Theater Theater { get; set; } = null!;

        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;
    }
}
