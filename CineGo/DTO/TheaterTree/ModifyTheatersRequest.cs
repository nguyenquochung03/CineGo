namespace CineGo.DTO.TheaterTree
{
    public class ModifyTheatersRequest
    {
        public int ShowtimeId { get; set; }
        public List<int> TheaterIds { get; set; } = new();
    }
}
