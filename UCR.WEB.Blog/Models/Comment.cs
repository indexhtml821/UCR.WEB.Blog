namespace UCR.WEB.Blog.Models;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public string UserName { get; set; }
    public DateTime CreationDateTime { get; set; }
}