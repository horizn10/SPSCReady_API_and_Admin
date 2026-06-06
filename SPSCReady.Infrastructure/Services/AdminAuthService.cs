using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using System.Security.Cryptography;

namespace SPSCReady.Infrastructure.Services;

public class AdminAuthService : IAdminAuthService
{
    private readonly ApplicationDbContext _db;

    public AdminAuthService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<AdminUser?> ValidateAsync(string username, string password)
    {
        var admin = await _db.AdminUsers
            .FirstOrDefaultAsync(a => a.Username == username && a.IsActive);

        if (admin == null) return null;

        return VerifyPassword(password, admin.PasswordHash) ? admin : null;
    }

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt,
            KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32));
        return $"{Convert.ToBase64String(salt)}.{hash}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt,
            KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32));

        return hash == parts[1];
    }
}