using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.Models.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogDatabase")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<BlogDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


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

app.Run();
