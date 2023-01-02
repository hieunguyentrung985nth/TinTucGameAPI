using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models2
{
    public partial class MessNotification
    {
        public string Id { get; set; } = null!;
        public string? MessId { get; set; }
        public string? OwnerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? Read { get; set; }

        public virtual Message? Mess { get; set; }
        public virtual staff? Owner { get; set; }
    }
}
