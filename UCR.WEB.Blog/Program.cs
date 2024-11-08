using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogDatabase")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<BlogDbContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
});




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var roles = new[] { "Author", "Administrator" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }





    var normEmail = userManager.NormalizeEmail("admin@gmail.com");
    var adminUser = await userManager.FindByEmailAsync(normEmail);

    if (adminUser == null)
    {
        // Create a new admin user if one doesn't exist
        adminUser = new User
        {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            Name = "admin", // You can change this as needed
            Password= "@Admin17"
        };

        var createResult = await userManager.CreateAsync(adminUser, "@Admin17"); // Set a default password

        if (createResult.Succeeded)
        {
            // Assign the Administrator role
            await userManager.AddToRoleAsync(adminUser, "Administrator");
            Console.WriteLine($"Admin user created and assigned 'Administrator' role: {adminUser.Email}");
        }
        else
        {
            Console.WriteLine("Error creating admin user: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }
    }
    else
    {
        Console.WriteLine($"Admin user already exists: {adminUser.Email}");
        // Check if the user already has the Administrator role
        if (!await userManager.IsInRoleAsync(adminUser, "Administrator"))
        {
            await userManager.AddToRoleAsync(adminUser, "Administrator");
            Console.WriteLine($"Assigned 'Administrator' role to existing user: {adminUser.Email}");
        }
    }


}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.Run();
