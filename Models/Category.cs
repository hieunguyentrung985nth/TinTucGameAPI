using System;
using System.Collections.Generic;

namespace TinTucGameAPI.Models
{
    public partial class Category
    {
        public Category()
        {
            InverseCategoryNavigation = new HashSet<Category>();
            Posts = new HashSet<Post>();
        }

        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Categoryid { get; set; }

        public virtual Category? CategoryNavigation { get; set; }
        public virtual ICollection<Category> InverseCategoryNavigation { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
