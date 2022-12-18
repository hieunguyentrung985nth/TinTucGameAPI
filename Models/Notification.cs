namespace TinTucGameAPI.Models
{
    public class Notification
    {
        public string? Id { get; set; } = null;
        public string? CreatorId { get; set; }
        public string? Type { get; set; }
        public string? PostId { get; set; }
        public string? OwnerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Read { get; set; }
    }
}
