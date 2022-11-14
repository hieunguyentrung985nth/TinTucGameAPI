using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models
{
    public partial class staff
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Phone { get; set; }
        public string? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
