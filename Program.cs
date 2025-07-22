using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using test2.Models;
using test2.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using test2.Models.ManagementModels.Services;

var facebookAppId = Environment.GetEnvironmentVariable("FACEBOOK_APP_ID");
var facebookAppSecret = Environment.GetEnvironmentVariable("FACEBOOK_APP_SECRET");
var googleClentId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
var googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

var builder = WebApplication.CreateBuilder(args);

#region ���U�ۭq�A�� �M �W�[�~�����ҳ]�w
// ���U ActivityService
builder.Services.AddScoped<ActivityService>();

// ���U AnnouncementService
builder.Services.AddScoped<AnnouncementService>();

// ���U AnnouncementService
builder.Services.AddScoped<UserService>();

// cookie service(Google ���ҳ]�w)
builder.Services.AddAuthentication(options => // �ק�G�N AddAuthentication �վ㬰���� options �e��
{
    // �]�w�w�]�����Ҥ�׬� Cookie �{��
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // �]�w�w�]���D�Ԥ�׬� Google ���ҡA���ݭn�~���n�J�ɷ|�ϥ�
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})

.AddGoogle(googleOptions => // �s�W�GGoogle ���ҳ]�w
{
    googleOptions.ClientId = googleClentId!;
    googleOptions.ClientSecret = googleClientSecret!;

    // �]�w�n�V Google �ШD���ϥΪ̸�ƽd��
    // "profile" �]�t�m�W�B�Y�����򥻸��
    // "email" �]�t�ϥΪ̪��q�l�l��a�}
    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");
})

.AddFacebook(facebookOptions => // �s�W�GFacebook ���ҳ]�w
{
    facebookOptions.AppId = facebookAppId!;
    facebookOptions.AppSecret = facebookAppSecret!;

    // �u�n�D���}�ӤH��� (�]�t�m�W)�C
    // Facebook �� 'public_profile' �d��w�]�]�t�m�W�C
    facebookOptions.Scope.Add("public_profile");
    facebookOptions.Scope.Add("email");
});
#endregion

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

//�D�n�O���w���Ƶ{
builder.Services.AddHostedService<ScheduleServices>();

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
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;
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