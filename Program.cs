using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using test2.Models;

var builder = WebApplication.CreateBuilder(args);

//database service
builder.Services.AddDbContext<Test2Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Test2ConnString")));

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

//memory service
builder.Services.AddDistributedMemoryCache();

//session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//cookie service
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
    options.LoginPath = "/Frontend/Home/Index";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapStaticAssets();

//session start
app.UseSession();

//cookie start
app.UseAuthentication();
app.UseAuthorization();

//fronted area
app.MapControllerRoute(
    name: "FrontendArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Frontend" }
);

//backed area
app.MapControllerRoute(
    name: "BackendArea",
    pattern: "{area:exists}/{controller=Manage}/{action=Index}/{id?}",
    defaults: new { area = "Backend" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();

app.Run();