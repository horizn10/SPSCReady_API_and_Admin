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
        public DbSet<ExamStage> ExamStages { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ExamPaper> ExamPapers { get; set; }
        public DbSet<ExamPaperDept> ExamPaperDepartments { get; set; }
        public DbSet<ExamPaperPost> ExamPaperPosts { get; set; }
        public DbSet<ExamPaperStage> ExamPaperStages { get; set; }
        public DbSet<ExamPaperSubject> ExamPaperSubjects { get; set; }

        // MockTest Module DbSets
        public DbSet<Exam> Exams { get; set; }
        public DbSet<MockTest> MockTests { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserAttempt> UserAttempts { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        // OTP Module DbSet
        public DbSet<OtpToken> OtpTokens { get; set; }

        // ── Admin Panel ───────────────────────────────────────────────────────
        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all entity configurations from this assembly
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Post -> Department
            builder.Entity<Post>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Posts)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPaper junctions
            // ExamPaperDept
            builder.Entity<ExamPaperDept>()
                .HasOne(d => d.Exam)
                .WithMany(ep => ep.Departments)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamPaperDept>()
                .HasIndex(d => new { d.ExamId, d.DepartmentId })
                .IsUnique();

            builder.Entity<ExamPaperDept>()
                .HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPaperPost
            builder.Entity<ExamPaperPost>()
                .HasOne(p => p.Exam)
                .WithMany(ep => ep.Posts)
                .HasForeignKey(p => p.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamPaperPost>()
                .HasIndex(p => new { p.ExamId, p.PostId })
                .IsUnique();

            builder.Entity<ExamPaperPost>()
                .HasOne(p => p.Post)
                .WithMany()
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPaperStage
            builder.Entity<ExamPaperStage>()
                .HasOne(s => s.Exam)
                .WithMany(ep => ep.Stages)
                .HasForeignKey(s => s.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamPaperStage>()
                .HasIndex(s => new { s.ExamId, s.StageId })
                .IsUnique();

            builder.Entity<ExamPaperStage>()
                .HasOne(s => s.Stage)
                .WithMany()
                .HasForeignKey(s => s.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPaperSubject leaf
            builder.Entity<ExamPaperSubject>()
                .HasOne(s => s.ExamPaper)
                .WithMany(ep => ep.Subjects)
                .HasForeignKey(s => s.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamPaperSubject>()
                .HasOne(s => s.Stage)
                .WithMany()
                .HasForeignKey(s => s.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamPaperSubject>()
                .HasOne(s => s.Subject)
                .WithMany()
                .HasForeignKey(s => s.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── AdminUser ─────────────────────────────────────────────────────
            builder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Username).HasMaxLength(50).IsRequired();
                entity.Property(a => a.PasswordHash).IsRequired();
                entity.Property(a => a.FullName).HasMaxLength(100).IsRequired();
                entity.HasIndex(a => a.Username).IsUnique();
            });
        }
    }
}