using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Configurations;

public class MockTestConfiguration : IEntityTypeConfiguration<MockTest>
{
    public void Configure(EntityTypeBuilder<MockTest> builder)
    {
        builder.ToTable("MockTests");
        builder.HasKey(x => x.MockTestId);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.PaperType)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(x => x.PaperNumber)
               .IsRequired();

        builder.Property(x => x.DurationMinutes)
               .IsRequired();

        builder.Property(x => x.TotalMarks)
               .IsRequired();

        builder.Property(x => x.TotalQuestions)
               .IsRequired();

        builder.Property(x => x.PassingMarks)
               .HasColumnType("decimal(6,2)");

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.Exam)
               .WithMany(e => e.MockTests)
               .HasForeignKey(x => x.ExamId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ExamId);
    }
}
