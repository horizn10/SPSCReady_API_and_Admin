using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Data
{
    // Inherit from IdentityDbContext instead of DbContext
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // You don't even need to explicitly add DbSet<ApplicationUser> here, 
        // IdentityDbContext handles the 'AspNetUsers' table automatically!
    }
}