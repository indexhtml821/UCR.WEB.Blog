using Microsoft.AspNetCore.Identity;

namespace UCR.WEB.Blog.Models;

public class User:IdentityUser
{
    public string Name { get; set; }
    public string? Role { get; set; }
    public string Password { get; set; }

}
