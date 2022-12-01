using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models
{
    public partial class User
    {
        public User()
        {
            Feeds = new HashSet<Feed>();
            Posts = new HashSet<Post>();
            staff = new HashSet<staff>();
            Roles = new HashSet<Role>();
        }

        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? Passwordhash { get; set; }
        public string? Salt { get; set; }
        public string? Avatar { get; set; }

        public virtual ICollection<Feed> Feeds { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<staff> staff { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
