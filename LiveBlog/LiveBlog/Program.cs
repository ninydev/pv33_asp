using LiveBlog.Areas.Sse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LiveBlog.Data;
using LiveBlog.Data.Seeds;
using LiveBlog.Models.IdentityUser;
using LiveBlog.Services.Storage;
using LiveBlog.Repositories.Base;
using LiveBlog.Repositories.Posts;
using LiveBlog.Repositories.Likes;
using LiveBlog.Services;
using LiveBlog.Services.Chat;
using LiveBlog.Services.Posts;
using LiveBlog.Services.Likes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<MyIdentityUserEntity>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddHttpContextAccessor();
// Реєстрація DI для репозиторіїв та сервісів
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostLikeRepository, PostLikeRepository>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<SseService>();
builder.Services.AddSingleton<ChatService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
await app.SeedDataAsync();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();