using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models2
{
    public partial class Room
    {
        public Room()
        {
            Messages = new HashSet<Message>();
            Users = new HashSet<staff>();
        }

        public string Id { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string? Name { get; set; }
        public bool? Group { get; set; }
        public string? LatestId { get; set; }

        public virtual Message? Latest { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<staff> Users { get; set; }
    }
}
