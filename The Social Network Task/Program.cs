using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using The_Social_Network_Task.DAL;
using The_Social_Network_Task.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(c =>
{
    c.AccessDeniedPath = "/AccessDenied";
});
builder.Services.AddMvc(m => m.EnableEndpointRouting = false);

var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
