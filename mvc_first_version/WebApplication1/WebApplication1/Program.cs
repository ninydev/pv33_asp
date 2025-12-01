using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.Seeds;
using WebApplication1.Entities;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Areas.MyTask.Repositories;
using WebApplication1.Areas.MyTask.Services;
using WebApplication1.Sse;

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

builder.Services.AddControllersWithViews()
//    .AddJsonOptions(opt =>
//    {
//        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//    })
    ;


// Регистрация репозитория
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
// Регистрация сервиса
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddSingleton<SseService>();


var app = builder.Build();


// Role Based Authorization
await app.SeedDataAsync();


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

// Маршрутизация для Areas (например, Area "Task")
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Основной маршрут по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Add a test route
app.MapGet("/test-route", () => "Hello from a test route!");

app.Run();