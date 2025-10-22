namespace CineGo.DTO.TheaterTree
{
    public class RegionNodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TotalCount { get; set; }
        public int SelectedCount { get; set; }
        public List<CityNodeDto> Cities { get; set; } = new();
    }
}
