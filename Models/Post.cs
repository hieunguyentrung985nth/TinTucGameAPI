using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models
{
    public partial class Post
    {
        public Post()
        {
            Categories = new HashSet<Category>();
            Images = new HashSet<Image>();
        }

        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? Author { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? AuthorNavigation { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
