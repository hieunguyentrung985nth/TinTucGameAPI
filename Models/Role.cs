using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TinTucGameAPI.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }
        [Key]
        public string Id { get; set; } = null!;
        public string? Role1 { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
