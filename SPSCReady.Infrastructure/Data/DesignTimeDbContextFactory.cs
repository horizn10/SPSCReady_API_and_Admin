using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SPSCReady.Infrastructure.Data;

namespace SPSCReady.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=SPSCReadyDb;Trusted_Connection=true;TrustServerCertificate=true;");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
