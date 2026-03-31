using Microsoft.AspNetCore.Identity;
using System;

namespace SPSCReady.Domain.Entities
{
    // Inheriting from IdentityUser gives us Email, PhoneNumber, PasswordHash, etc. for free
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}