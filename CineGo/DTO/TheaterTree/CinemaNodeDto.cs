namespace CineGo.DTO.TheaterTree
{
    public class CinemaNodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TotalCount { get; set; }
        public int SelectedCount { get; set; }
        public List<TheaterNodeDto> Theaters { get; set; } = new();
    }
}
