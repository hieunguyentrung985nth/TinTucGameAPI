using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models2
{
    public partial class Notification
    {
        public string Id { get; set; } = null!;
        public string? CreatorId { get; set; }
        public string? Type { get; set; }
        public string? PostId { get; set; }
        public string? OwnerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Read { get; set; }
    }
}
