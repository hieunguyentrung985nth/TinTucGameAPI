namespace TinTucGameAPI.Models
{
    public class Message
    {
        public string? Id { get; set; }
        public string SenderId { get; set; }
        public string RoomId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Content { get; set; }
    }
}
