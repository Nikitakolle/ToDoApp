using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Connect to your database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Identity (authentication system)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. Tell the app where to send users who are not logged in
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Your login page URL
});

// 4. Add support for MVC (controllers and views)
builder.Services.AddControllersWithViews();

// 5. Require users to be logged in everywhere by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// 6. Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// 7. Set default page to your ToDoes controller's Index action (your ToDo list)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ToDoes}/{action=Index}/{id?}");

app.Run();
