using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using SPSCReady.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 1. Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // You can customize password requirements here if you want!
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. Register your Application Services
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

// ... existing middleware ...

app.UseHttpsRedirection();

// Add Authentication middleware BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();