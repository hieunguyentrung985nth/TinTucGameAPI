using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models
{
    public partial class Feed
    {
        public string? Id { get; set; } = null!;
        public string? Content { get; set; }
        public string? UserId { get; set; }
        public string? PostId { get; set; }

        public virtual Post? Post { get; set; }
        public virtual User? User { get; set; }
    }
}
