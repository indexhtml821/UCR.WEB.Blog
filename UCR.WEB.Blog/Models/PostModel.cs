namespace UCR.WEB.Blog.Models;

public class PostUserModel
{
      public IEnumerable<Post> Posts { get; set; }
      public IEnumerable<User> Users { get; set; }
}