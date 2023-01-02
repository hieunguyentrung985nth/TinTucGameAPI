using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models2
{
    public partial class staff
    {
        public staff()
        {
            MessNotifications = new HashSet<MessNotification>();
            Messages = new HashSet<Message>();
            Rooms = new HashSet<Room>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Phone { get; set; }
        public string? UserId { get; set; }

        public virtual ICollection<MessNotification> MessNotifications { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
