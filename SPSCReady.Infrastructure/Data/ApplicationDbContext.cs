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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
        }
    }
}
