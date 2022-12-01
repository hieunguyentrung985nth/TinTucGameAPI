using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public string Id { get; set; } = null!;
        public string? Role1 { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
