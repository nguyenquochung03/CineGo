namespace CineGo.DTO.TheaterTree
{
    public class CityNodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TotalCount { get; set; }
        public int SelectedCount { get; set; }
        public List<CinemaNodeDto> Cinemas { get; set; } = new();
    }
}
