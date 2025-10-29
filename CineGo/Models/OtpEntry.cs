namespace CineGo.Models
{
    public class OtpEntry
    {
        public string Code { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
