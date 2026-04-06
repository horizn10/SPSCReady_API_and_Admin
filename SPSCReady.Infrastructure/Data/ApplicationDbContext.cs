using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ExamCycle> ExamCycles { get; set; }
        public DbSet<ExamStage> ExamStages { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ExamPaper> ExamPapers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Post -> Department
            builder.Entity<Post>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Posts)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. ExamCycle -> Post
            builder.Entity<ExamCycle>()
                .HasOne(ec => ec.Post)
                .WithMany(p => p.ExamCycles)
                .HasForeignKey(ec => ec.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. ExamCycle -> Department
            builder.Entity<ExamCycle>()
                .HasOne(ec => ec.Department)
                .WithMany(d => d.ExamCycles)
                .HasForeignKey(ec => ec.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. ExamPaper -> ExamCycle, Stage, and Subject
            builder.Entity<ExamPaper>()
                .HasOne(ep => ep.ExamCycle)
                .WithMany(ec => ec.ExamPapers)
                .HasForeignKey(ep => ep.ExamCycleId)
                .OnDelete(DeleteBehavior.Cascade); // Safe to cascade delete papers if a cycle is removed

            builder.Entity<ExamPaper>()
                .HasOne(ep => ep.ExamStage)
                .WithMany(es => es.ExamPapers)
                .HasForeignKey(ep => ep.ExamStageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamPaper>()
                .HasOne(ep => ep.Subject)
                .WithMany(s => s.ExamPapers)
                .HasForeignKey(ep => ep.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional: Create the Composite Index for lightning-fast searches
            builder.Entity<ExamCycle>()
                .HasIndex(ec => new { ec.DepartmentId, ec.PostId, ec.ExamYear });
        }
    }
}