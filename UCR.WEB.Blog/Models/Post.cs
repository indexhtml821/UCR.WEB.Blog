namespace UCR.WEB.Blog.Models;

public class Post
{
    public Post() { Comments = new List<Comment>(); }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public string UserId { get; set; }
    public byte[]? ImageData { get; set; }
    public string Category { get; set; }
}
