﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.Models.Data;
using UCR.WEB.Blog.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace UCR.WEB.Blog.Controllers;

public class AccountController : Controller
{
    private readonly BlogDbContext _context;
    public AccountController(BlogDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserVM model)
    {
        if (model.Password != model.ConfirmationKey)
        {
            ViewData["Error"] = "Passwords do not match";
            return View();
        }

        User user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Password = model.Password,
            Role = "Author"
        };

        if (ModelState.IsValid)
        {
           await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        if(user.Id != null)return RedirectToAction("Index", "Home");
        ViewData["Error"] = "User could not be created";

        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model)
    {

        User? foundUser  = await _context.User.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefaultAsync();

        if (foundUser == null)
        {

            ViewData["Error"] = "User not found";
            return View();
        }
        List<Claim>claims = new List<Claim>(){
            new Claim(ClaimTypes.Name , foundUser.Name)

        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        AuthenticationProperties properties = new AuthenticationProperties
        {
            AllowRefresh = true,
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

        return RedirectToAction("Index", "Home");
    }

}