using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column("UserName")]
        public string? Email { get; set; }
        public string? Passwordhash { get; set; }
        public string? Salt { get; set; }
        public string? Avatar { get; set; }
        public int? AccessFailedCount { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public Boolean? LockoutEnabled { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? NormalizedEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public Boolean? PhoneNumberConfirmed { get; set; }
        public Boolean? TwoFactorEnabled { get; set; }
        public string? SecurityStamp { get; set; }

        public virtual ICollection<Feed> Feeds { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<staff> staff { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
