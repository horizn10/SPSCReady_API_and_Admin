using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.Interfaces;
using SPSCReady.Infrastructure.Data;
using SPSCReady.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment(); // ← read env before Build()

builder.Services.AddControllersWithViews();

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Cloudflare R2 storage ─────────────────────────────────────────────────────
builder.Services.AddSingleton<IR2StorageService, R2StorageService>();

// ── Admin Auth ────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();

// ── Cookie Authentication ─────────────────────────────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "AdminCookie";
    options.DefaultChallengeScheme = "AdminCookie";
    options.DefaultSignInScheme = "AdminCookie";
})
.AddCookie("AdminCookie", options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.Name = "AdminSession";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = isDevelopment
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;
});

var app = builder.Build();

// ── Seed admin users ──────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (!db.AdminUsers.Any())
        {
            db.AdminUsers.AddRange(
                new SPSCReady.Domain.Entities.AdminUser
                {
                    Username = "admin1",
                    PasswordHash = AdminAuthService.HashPassword("Admin@123"),
                    FullName = "Administrator One",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new SPSCReady.Domain.Entities.AdminUser
                {
                    Username = "admin2",
                    PasswordHash = AdminAuthService.HashPassword("Admin@456"),
                    FullName = "Administrator Two",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new SPSCReady.Domain.Entities.AdminUser
                {
                    Username = "admin3",
                    PasswordHash = AdminAuthService.HashPassword("Admin@789"),
                    FullName = "Administrator Three",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
            db.SaveChanges();
            Console.WriteLine("[Seed] 3 admin users inserted.");
        }
        else
        {
            Console.WriteLine("[Seed] Admin users already exist, skipping.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Seed Error] {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // ← must be before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();