using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Configurations;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("MockExams");
        builder.HasKey(x => x.ExamId);

        builder.Property(x => x.ExamName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.ExamCode)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(x => x.ExamCode)
               .IsUnique();

        builder.Property(x => x.ExamYear)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(-1);  // nvarchar(max)

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
    }
}
