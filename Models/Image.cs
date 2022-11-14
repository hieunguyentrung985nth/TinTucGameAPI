using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models
{
    public partial class Image
    {
        public Image()
        {
            Posts = new HashSet<Post>();
        }

        public string Id { get; set; } = null!;
        public string? Image1 { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
