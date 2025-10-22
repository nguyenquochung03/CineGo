namespace CineGo.DTO.TheaterTree
{
    public class NodeIdRequest
    {
        public string NodeType { get; set; } = null!;
        public int NodeId { get; set; }
        public int ShowtimeId { get; set; }
    }
}
