namespace TinTucGameAPI.Models
{
    public class MessNotifications
    {
        public string Id { get; set; }
        public string MessId { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Read { get; set; }
    }
}
