namespace UCR.WEB.Blog.Models
{
      public class BlogViewModel
      {
            public IEnumerable<Post> Posts { get; set; }
            public IEnumerable<User> Users { get; set; }
      }
}