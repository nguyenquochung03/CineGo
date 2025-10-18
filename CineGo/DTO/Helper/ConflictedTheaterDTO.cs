namespace CineGo.DTO.Helper
{
    public class ConflictedTheaterDTO
    {
        public int TheaterId { get; set; }
        public string TheaterName { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public int ConflictingShowtimeId { get; set; }
        public TimeSpan ConflictingStart { get; set; }
        public TimeSpan ConflictingEnd { get; set; }
    }
}
