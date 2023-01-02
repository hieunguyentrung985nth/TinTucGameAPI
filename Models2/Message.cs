using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models2
{
    public partial class Message
    {
        public Message()
        {
            MessNotifications = new HashSet<MessNotification>();
            Rooms = new HashSet<Room>();
        }

        public string Id { get; set; } = null!;
        public string? SenderId { get; set; }
        public string? RoomId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Content { get; set; }

        public virtual Room? Room { get; set; }
        public virtual staff? Sender { get; set; }
        public virtual ICollection<MessNotification> MessNotifications { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
